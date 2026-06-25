using UnityEngine;

namespace Ceduc.SWMiningTruck
{
    [DisallowMultipleComponent]
    public class GroundSnapper : MonoBehaviour
    {
        [Header("Ground Snap Settings")]

        // La llamada a la función TrySnapToGround se hace en modo edit. Si se quisiera hacer en el Awake hay que activar esta variable e implementar la función Awake().
        //[Tooltip("Si está activo, el objeto ajustará su posición Y al tocar el suelo al iniciar.")]
        //[SerializeField] private bool snapOnAwake = false;

        [Tooltip("Si está activo, el objeto ajustará su posición sólo hacia abajo. En caso que ya esté un poco bajo el suelo, no se cambiará su posición.")]
        [SerializeField] private bool onlySnapDownward = true;

        [Tooltip("Distancia máxima que se buscará hacia abajo para detectar el suelo.")]
        [SerializeField] private float maxRayDistance = 10f;

        [Tooltip("Desplazamiento adicional sobre el punto de contacto con el suelo.")]
        [SerializeField] private float yOffset = 0f;

        [Tooltip("Capas consideradas como suelo.")]
        [SerializeField] private LayerMask groundLayer = ~0; // Por defecto, todas las capas.
        
        /// <summary>
        /// Ajusta la posición Y del objeto para que toque el suelo bajo él.
        /// </summary>
        public void TrySnapToGround()
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f; // Un poco arriba para evitar autocolisión.
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxRayDistance, groundLayer))
            {
                Vector3 newPos = transform.position;
                newPos.y = hit.point.y + yOffset;

                if (onlySnapDownward && newPos.y > transform.position.y)
                {
                    return;
                }

                transform.position = newPos;
            }
            //else
            //{
            //    Debug.LogWarning($"{name}: No se detectó suelo bajo el objeto en el rango de {maxRayDistance}m.", this);
            //}
        }
    }

}