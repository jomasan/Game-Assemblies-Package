using UnityEngine;

/// Runtime policy manager: single source of truth for "the current law" used by the ownership model.
/// Assign a PolicyDataSO in the Inspector, or leave empty to use default policy (no taxation in this base implementation).
public class PolicyManager : MonoBehaviour
{
    public static PolicyManager Instance { get; private set; }

    [Header("Current Policy")]
    [Tooltip("Active policy asset. If null, default policy is used (PrivateIndividual, Disallowed stealing, ResourceOwner attribution, Anyone station use).")]
    public PolicyDataSO policy;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// Current ownership model (communal, per-player, per-team, or hybrid).
    public OwnershipModel GetOwnershipModel()
    {
        return policy != null ? policy.ownershipModel : OwnershipModel.PrivateIndividual;
    }

    /// Whether the actor is allowed to take a resource currently owned by currentOwner. Returns true if currentOwner is null (unowned/common).
    public bool CanTakeResource(playerController actor, playerController currentOwner)
    {
        if (currentOwner == null)
            return true;

        if (actor == null)
            return false;

        if (actor == currentOwner)
            return true;

        switch (policy != null ? policy.stealingPolicy : StealingPolicy.Disallowed)
        {
            case StealingPolicy.Allowed:
            case StealingPolicy.Penalized:
                return true;
            case StealingPolicy.Disallowed:
            default:
                return false;
        }
    }

    /// Whether taking this resource would be considered stealing (actor is not the owner). Used to apply penalties when policy is Penalized.
    public bool IsTakingStealing(playerController actor, playerController currentOwner)
    {
        if (currentOwner == null || actor == null || actor == currentOwner)
            return false;
        return true;
    }

    /// Who gets credit when a goal is completed.
    public GoalAttribution GetGoalAttribution()
    {
        return policy != null ? policy.goalAttribution : GoalAttribution.ResourceOwner;
    }

    /// When attribution is Split, share of credit for the resource owner (0–1).
    public float GetGoalAttributionOwnerShare()
    {
        return policy != null ? policy.goalAttributionOwnerShare : 0.7f;
    }

    /// Whether the actor can use a station owned by stationOwner. SameTeam currently treats same player as same team until team IDs exist.
    public bool CanUseStation(playerController actor, playerController stationOwner)
    {
        StationUsePolicy usePolicy = policy != null ? policy.stationUsePolicy : StationUsePolicy.Anyone;

        switch (usePolicy)
        {
            case StationUsePolicy.OwnerOnly:
                return stationOwner == null || actor == stationOwner;
            case StationUsePolicy.SameTeam:
                if (stationOwner == null) return true;
                if (actor == null) return false;
                // TODO: when team IDs exist, compare actor.teamId == stationOwner.teamId
                return actor == stationOwner;
            case StationUsePolicy.Anyone:
            case StationUsePolicy.AnyoneWithFee:
                return true;
            default:
                return true;
        }
    }

    /// Whether voluntary transfer of resources/capital between players is allowed.
    public bool IsSharingAllowed()
    {
        return policy == null || policy.sharingAllowed;
    }

    /// Whether other players can see a player's resources/capital.
    public ResourceVisibility GetResourceVisibility()
    {
        return policy != null ? policy.resourceVisibility : ResourceVisibility.Public;
    }

    /// Set the active policy at runtime (e.g., when players change the law).
    public void SetPolicy(PolicyDataSO newPolicy)
    {
        policy = newPolicy;
    }
}
