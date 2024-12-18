using System;
using Detection;
using UnityEngine;
using UnityEngine.AI;


// Phoebe Faith

///<summary>
/// Contains code relating to NavMesh AI stuff
/// </summary>


namespace Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MonoBehaviour
    {
        public enum AIType
        {
            npc,
            enemy
        }

        public class AIControllerDebugData
        {
            public AIType aiType;
            public Transform target;
            public BasePlayer targetPlayer;
            public BasePlayer player;
        }
        
        public Action<AIControllerDebugData> OnAIUpdate;
        
        private AIControllerDebugData _debugData = new AIControllerDebugData();
        
        public AIType aiType = AIType.npc;
        [SerializeField]
        private DetectionMiddleman detectionMiddleman;
        
        private NavMeshAgent agent;
        private BasePlayer player;

        private BasePlayer targetPlayer;
        private Transform target;

        public Transform patrolTarget;

        public Action<AIController> PatrolTargetReached;
        private bool _patrolTargetLock = false;

        public float patrolThreshold = 1.25f;
        private void Awake()
        {
            // Find player and navMeshAgent
            TryGetComponent<NavMeshAgent>(out agent);
            if(agent == null) throw new Exception("NavMeshAgent component is null");
            TryGetComponent<BasePlayer>(out player);
            if(player == null) throw new Exception("BasePlayer component is null");
            
            // Set debugData
            _debugData.aiType = aiType;
            _debugData.player = player;
        }

        // Hook into detectionMiddleman
        private void OnEnable()
        {
            detectionMiddleman.Detected += DetectionMiddlemanOnDetected;
            detectionMiddleman.DetectedClear += DetectedClear;
        }

        // Unhook from detectionMiddleman
        private void OnDisable()
        {
            detectionMiddleman.DetectedClear -= DetectedClear;
            detectionMiddleman.Detected -= DetectionMiddlemanOnDetected;
        }

        private void DetectedClear()
        {
            // If clear event triggered, unassign all and send debug update.            
            targetPlayer = null;
            target = null;
            _debugData.target = null;
            _debugData.targetPlayer = null;
            OnAIUpdate?.Invoke(_debugData);
        }

        private void Update()
        {
            // If there is a player that is currently a target, set the destination to their position at all times.
            if(targetPlayer != null) agent.SetDestination(targetPlayer.transform.position);
            else if (target != null) agent.SetDestination(target.position);
            else if(patrolTarget != null) agent.SetDestination(patrolTarget.position);

            if (patrolTarget)
            {
                float distance = (patrolTarget.position - transform.position).magnitude; // Distance between target and transform
                
                if (distance <= patrolThreshold) // If in threshold
                {
                    if (_patrolTargetLock == false) // If not locked
                    {
                        _patrolTargetLock = true; // Lock
                        PatrolTargetReached?.Invoke(this); // Call event
                    }                
                }
            }
        }
        
        public AIControllerDebugData GetDebugData()
        {
            // Obsolete
            return null;
        }

        public void SetNewPatrolTarget(Transform newTarget)
        {
            patrolTarget = newTarget; // Set new target
            _patrolTargetLock = false; // Unlock
        }
        
        private void DetectionMiddlemanOnDetected(object sender, DetectionBase.DetectionEventData data)
        {
            if (data.player)
            {
                // Update NavMeshAgent target
                agent.SetDestination(data.player.transform.position);
                // Set variables
                targetPlayer = data.player;
                _debugData.targetPlayer = targetPlayer;
                // Update event
                OnAIUpdate?.Invoke(_debugData);
            }
            else if(data.position)
            {
                // Update NavMeshAgent target
                agent.SetDestination(data.position.position);
                // Set variables
                target = data.position;
                _debugData.target = target;
                // Update event
                OnAIUpdate?.Invoke(_debugData);
            }
        }
    }
}