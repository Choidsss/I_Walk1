using UnityEngine;
using Newtonsoft.Json;

namespace UGESystem
{
    /// <summary>
    /// An abstract base class for all event rewards,
    /// defining the execution flow and the <see cref="OnGrantReward"/> method that must be implemented by concrete reward types.
    /// This class uses the Template Method pattern to ensure consistent duplicate-prevention logic.
    /// </summary>
    [System.Serializable]
    public abstract class AbstractEventReward
    {
        [field: SerializeField]
        [JsonProperty]
        /// <summary>
        /// A unique identifier for this reward instance, used to prevent duplicate granting during save/load cycles.
        /// </summary>
        public string RewardID { get; private set; }

        [field: SerializeField]
        [JsonProperty]
        /// <summary>
        /// Gets the human-readable description of this reward.
        /// </summary>
        public string Description { get; private set; }

        [field: SerializeField]
        [JsonProperty]
        /// <summary>
        /// If true, this reward can only be granted once per game session (based on RewardID).
        /// </summary>
        public bool IsOneTimeOnly { get; private set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventReward"/> class.
        /// </summary>
        public AbstractEventReward() 
        {
            EnsureID();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventReward"/> class with a specified description.
        /// </summary>
        /// <param name="description">A human-readable description of the reward.</param>
        public AbstractEventReward(string description)
        {
            Description = description;
            EnsureID();
        }

        private void EnsureID()
        {
            if (string.IsNullOrEmpty(RewardID))
            {
                RewardID = System.Guid.NewGuid().ToString();
            }
        }

        /// <summary>
        /// Final entry point for granting the reward. 
        /// It handles duplicate prevention and then calls the implementation-specific <see cref="OnGrantReward"/>.
        /// </summary>
        /// <param name="runner">The <see cref="UGEEventTaskRunner"/> that is currently executing the event.</param>
        public void GrantReward(UGEEventTaskRunner runner)
        {
            // 1. Guard for One-Time Rewards
            if (IsOneTimeOnly && !string.IsNullOrEmpty(RewardID))
            {
                if (UGESystemController.Instance.IsRewardAlreadyGranted(RewardID))
                {
#if UNITY_EDITOR
                    Debug.Log($"[RewardSystem] Reward '{Description}' (ID: {RewardID}) was skipped because it was already granted.");
#endif
                    return;
                }
            }

            // 2. Delegate actual reward logic to subclasses
            OnGrantReward(runner);

            // 3. Register as granted if applicable
            if (IsOneTimeOnly && !string.IsNullOrEmpty(RewardID))
            {
                UGESystemController.Instance.RegisterGrantedReward(RewardID);
            }
        }

        /// <summary>
        /// Implemented by concrete reward types to define the actual logic of granting the reward.
        /// </summary>
        /// <param name="runner">The <see cref="UGEEventTaskRunner"/> that is currently executing the event.</param>
        protected abstract void OnGrantReward(UGEEventTaskRunner runner);

#if UNITY_EDITOR
        /// <summary>
        /// Assigns a new GUID if the RewardID is empty. Called by the editor.
        /// </summary>
        public void EDITOR_EnsureGuid()
        {
            EnsureID();
        }
#endif
    }
}