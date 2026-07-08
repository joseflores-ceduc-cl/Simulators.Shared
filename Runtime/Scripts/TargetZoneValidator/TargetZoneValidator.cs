using Ceduc.Shared;
using DG.Tweening;
using System;
using UnityEngine;

namespace Ceduc.SWMiningTruck
{

    public class TargetZoneValidator : MonoBehaviour
    {
        public enum ValidationState { Unset, Outside, Partial, Inside, Inactive }

        [Tooltip("El objeto a validar.")]
        [SerializeField] GameObject targetObject;
        [SerializeField] bool validateOrientation = true;
        [SerializeField] float orientationThresholdDegree = 20f;

        [Tooltip("Color de visualizaci�n para estado 'Outside'")]
        [SerializeField] Color outsideColor = Color.red;
        [Tooltip("Color de visualizaci�n para estado 'Partial'")]
        [SerializeField] Color partialColor = Color.yellow;
        [Tooltip("Color de visualizaci�n para estado 'Inside'")]
        [SerializeField] Color insideColor = Color.green;
        [SerializeField] Vector3 boundsExpansionDelta;
        [SerializeField] float minDistanceForPositioningTime = 30f;

        [Header("Frame")]
        [SerializeField] float frameThickness = 0.5f;
        [SerializeField] GameObject frameCubePrefab;

        [Header("Arrow")]
        [SerializeField] GameObject arrow;
        [SerializeField] float arrowInitialLocalZPosition = -8f;
        [SerializeField] float arrowEndLocalZPosition = 2f;
        [SerializeField] float arrowDurationAnimation = 1.5f;

        // Los eventos que declara este script son de instancia, no static, porque en una misma escena pueden haber varias zonas: una de carga y otra de descarga.        
        public event System.Action OnTargetProperlyPositioned;
        public event System.Action OnTargetLostProperPosition;

        public float DeltaAngle
        {
            get 
            {
                var a = transform.forward;
                var b = targetObject.transform.forward;
                return Vector3.SignedAngle(a, b, Vector3.up);
            }
        }
        public float DeltaLateral 
        { 
            get 
            {
                Vector3 localPosition = transform.InverseTransformPoint(targetObject.transform.position);
                return localPosition.x;
            } 
        }
        public float DeltaDepth
        {
            get
            {
                Vector3 localPosition = transform.InverseTransformPoint(targetObject.transform.position);
                return localPosition.z;
            }
        }


        BoxCollider zoneCollider;
        Bounds boundsRelativeToTarget;
        Vector3[] verticesRelativeToTarget;        
        GameObject frameParent;
        ValidationState currentState;
        bool calculatePositioningTime;
        float positioningTime;


        public ValidationState CurrentState 
        { 
            get => currentState;
            private set
            {
                // No se permite transicionar al mismo estado
                if (currentState == value)
                    return;

                //print($"From: {currentState} To: {value}");
                var oldState = currentState;

                switch (oldState)
                {
                    case ValidationState.Inactive:
                        InactiveExit(value);
                        break;
                    case ValidationState.Inside:
                        InsideExit(value); 
                        break;
                }                
                
                currentState = value;

                switch (currentState)
                {
                    case ValidationState.Outside:
                        OutsideEnter(oldState);
                        break;
                    case ValidationState.Partial:
                        PartialEnter(oldState);
                        break;
                    case ValidationState.Inside:
                        InsideEnter(oldState);
                        break;
                    case ValidationState.Inactive:
                        InactiveEnter(oldState);
                        break;
                }
            }
        }

        public ZoneValidatorData GetZoneValidatorData()
        {
            ZoneValidatorData data = new ZoneValidatorData 
            { 
                deltaAngle = DeltaAngle, 
                deltaDepth = DeltaDepth, 
                deltaLateral = DeltaLateral,
                positioningTime = this.positioningTime
            };

            return data;
        }

        void OutsideEnter(ValidationState oldState)
        {         
            UpdateZoneColor(outsideColor);
        }

        void PartialEnter(ValidationState oldState)
        {            
            UpdateZoneColor(partialColor);
        }

        void InsideEnter(ValidationState oldState)
        {            
            UpdateZoneColor(insideColor);
            OnTargetProperlyPositioned?.Invoke();
        }

        void InactiveEnter(ValidationState oldState)
        {            
            frameParent.SetActive(false);
            StopArrowAnimation();
            arrow.SetActive(false);
        }
        private void InactiveExit(ValidationState newState)
        {
            frameParent.SetActive(true);
            arrow.SetActive(true);
            calculatePositioningTime = false;
            positioningTime = 0f;
            PlayArrowAnimation();
        }

        private void InsideExit(ValidationState newState)
        {
            if (newState == ValidationState.Outside || newState == ValidationState.Partial)
            {
                OnTargetLostProperPosition?.Invoke();
            }
        }

        private void Awake()
        {
            if (targetObject == null)
            {
                targetObject = ComponentFinder.FindFirstGameObjectWithInterfaceOnTag<ITargetable>();
                if (targetObject == null)
                {
                    Debug.LogWarning("TargetZoneValidator: debe definir un valor para targetObject.");
                    return;
                }                
            }

            boundsRelativeToTarget = CommonUtils.GetCombinedRendererBounds(targetObject);
            verticesRelativeToTarget = CommonUtils.GetVerticesFromAABB(boundsRelativeToTarget.min, boundsRelativeToTarget.max);            
            zoneCollider = CreateBoxColliderForZone(boundsRelativeToTarget);
            frameParent = CreateFrame(zoneCollider, frameThickness);

            CurrentState = ValidationState.Unset;
            calculatePositioningTime = false;
        }

        private void OnDisable()
        {
            StopArrowAnimation();
        }

        private void Start()
        {
            PlayArrowAnimation();
        }

        void PlayArrowAnimation()
        {
            StopArrowAnimation();
            var initialPosition = arrow.transform.localPosition;
            initialPosition.z = arrowInitialLocalZPosition;
            arrow.transform.localPosition = initialPosition;
            arrow.transform.DOLocalMoveZ(arrowEndLocalZPosition, arrowDurationAnimation)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

        void StopArrowAnimation()
        {
            arrow.transform.DOKill();
        }

        private void Update()
        {
            DebugUpdate();

            if (targetObject == null || zoneCollider == null)
                return;

            if (CurrentState == ValidationState.Inactive)
                return;

            UpdateMetric_PositioningTime();

            ValidationState state = EvaluateTargetPositionAndOrientation();

            if (state != CurrentState)
            {
                CurrentState = state;                
            }
        }

        void UpdateMetric_PositioningTime()
        {
            if (calculatePositioningTime)
            {
                positioningTime += Time.deltaTime;
            }
            else if (Vector3.Distance(transform.position, targetObject.transform.position) <= minDistanceForPositioningTime)
            {
                calculatePositioningTime = true;
                positioningTime = 0f;
            }
        }

        void DebugUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
                string parentName = (transform.parent != null) ? transform.parent.name : "";
                print($"[{parentName}]DeltaLateral: {DeltaLateral}. DeltaDepth: {DeltaDepth}. DeltaAngle: {DeltaAngle}. Distance to target: {distanceToTarget}");
            }
        }

        public void SetActiveZone(bool value)
        {
            CurrentState = value ? ValidationState.Unset : ValidationState.Inactive;
        }

        BoxCollider CreateBoxColliderForZone(Bounds bounds)
        {                                    
            //print($"CreateBoxColliderForZone Min: {bounds.min}. Max: {bounds.max}");
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0f, bounds.extents.y, 0f); // Se asume que este transform est� con position.y en la base del suelo.
            boxCollider.size = bounds.size + boundsExpansionDelta; // Despu�s de centrarlo le incremento el tama�o: para que la pos.y quede algo debajo del suelo.
            boxCollider.isTrigger = true;                                                
            return boxCollider;
        }

        GameObject CreateFrame(BoxCollider boxCollider, float thickness)
        {
            float width = boxCollider.size.x;
            float height = boxCollider.size.y - boundsExpansionDelta.y / 2f;
            float depth = boxCollider.size.z;
            Vector3 center = transform.position + Vector3.up * height / 2f;

            GameObject parent = new GameObject("FrameParent");
            parent.transform.SetParent(transform);
            parent.transform.position = center;
            parent.transform.rotation = Quaternion.identity; // Luego de crear el frame se rotar� para que quede con la misma rotaci�n que el TargetZoneValidator.

            float w = width;
            float h = height;
            float d = depth;
            float t = thickness;

            float hw = w / 2f;
            float hh = h / 2f;
            float hd = d / 2f;

            float offsetX = hw - t / 2f;
            float offsetY = hh - t / 2f;
            float offsetZ = hd - t / 2f;

            // Aristas horizontales (eje X)
            CreateEdge(center, new Vector3(w, t, t), new Vector3(0, offsetY, offsetZ), parent.transform);
            CreateEdge(center, new Vector3(w, t, t), new Vector3(0, offsetY, -offsetZ), parent.transform);
            CreateEdge(center, new Vector3(w, t, t), new Vector3(0, -offsetY, offsetZ), parent.transform);
            CreateEdge(center, new Vector3(w, t, t), new Vector3(0, -offsetY, -offsetZ), parent.transform);

            // Aristas verticales (eje Y)
            CreateEdge(center, new Vector3(t, h, t), new Vector3(offsetX, 0, offsetZ), parent.transform);
            CreateEdge(center, new Vector3(t, h, t), new Vector3(offsetX, 0, -offsetZ), parent.transform);
            CreateEdge(center, new Vector3(t, h, t), new Vector3(-offsetX, 0, offsetZ), parent.transform);
            CreateEdge(center, new Vector3(t, h, t), new Vector3(-offsetX, 0, -offsetZ), parent.transform);

            // Aristas longitudinales (eje Z)
            CreateEdge(center, new Vector3(t, t, d), new Vector3(offsetX, offsetY, 0), parent.transform);
            CreateEdge(center, new Vector3(t, t, d), new Vector3(offsetX, -offsetY, 0), parent.transform);
            CreateEdge(center, new Vector3(t, t, d), new Vector3(-offsetX, offsetY, 0), parent.transform);
            CreateEdge(center, new Vector3(t, t, d), new Vector3(-offsetX, -offsetY, 0), parent.transform);

            parent.transform.rotation = transform.rotation;
            return parent;
        }

        void CreateEdge(Vector3 center, Vector3 scale, Vector3 offset, Transform parent)
        {
            if (frameCubePrefab == null)
            {
                Debug.LogError("No se asign� un prefab de cubo.");
                return;
            }

            GameObject edge = Instantiate(frameCubePrefab, center + offset, Quaternion.identity, parent);
            edge.transform.localScale = scale;
        }        

        ValidationState EvaluateTargetPositionAndOrientation()
        {                                    
            int totalValidations = verticesRelativeToTarget.Length;
            int validationsCount = 0;

            for (int i = 0; i < totalValidations; i++)
            {
                Vector3 vertexRelative = verticesRelativeToTarget[i];
                Vector3 vertexWorld = targetObject.transform.TransformPoint(vertexRelative);
                if (CommonUtils.IsPointInsideBoxCollider(zoneCollider, vertexWorld))
                {
                    validationsCount++;
                }
            }
            
            // La validaci�n de la orientaci�n la hacemos solo si es que ya pas� la validaci�n de posici�n.
            if (validateOrientation && validationsCount == totalValidations)
            {
                totalValidations++;
                if (Math.Abs(DeltaAngle) < orientationThresholdDegree)
                {
                    validationsCount++;
                }
            }

            if (validationsCount == 0)
                return ValidationState.Outside;
            else if (validationsCount == totalValidations)
                return ValidationState.Inside;
            else
                return ValidationState.Partial;            
        }
        
        void UpdateZoneColor(Color color)
        {                                    
            if (frameParent == null) 
                return;            

            foreach (Transform child in frameParent.transform)
            {
                child.GetComponent<Renderer>().material.color = color;
            }

            arrow.GetComponentInChildren<Renderer>().material.color = color;
        }        

    }

    [System.Serializable]
    public struct ZoneValidatorData
    {
        [UIName("Error de ángulo de posicionamiento"), UIFormat("F1", "°")]
        public float deltaAngle;
        [UIName("Error lateral de posicionamiento"), UIFormat("F1", "m")]
        public float deltaLateral;
        [UIName("Error en profundidad de posicionamiento"), UIFormat("F1", "m")]
        public float deltaDepth;
        [UIName("Tiempo de posicionamiento"), UITimeFormat]
        public float positioningTime;        
    }

}