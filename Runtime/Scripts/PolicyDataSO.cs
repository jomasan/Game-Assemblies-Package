using UnityEngine;

/// Defines how property is understood in the simulation.
/// Used by the ownership model and PolicyManager to determine counting and attribution.
public enum OwnershipModel
{
    /// No private ownership; all resources and stations are shared. Stealing is N/A.
    Communal,
    /// Each player owns their own resources and optionally stations.
    PrivateIndividual,
    /// Ownership is by team; resources/stations belong to a team.
    PrivateTeam,
    /// Some resources or zones are communal, others private (future: per-type or per-zone rules).
    Hybrid
}

/// Whether a player can take a resource owned by another.
public enum StealingPolicy
{
    Allowed,
    Disallowed,
    /// Taking is allowed but may trigger a cost or penalty (handled by game logic; taxation excluded for now).
    Penalized
}

/// Who gets credit when a goal is completed (e.g., resource delivered).
public enum GoalAttribution
{
    /// Credit goes to the owner of the resource that was delivered.
    ResourceOwner,
    /// Credit goes to the player who delivered the resource.
    Deliverer,
    /// Credit goes to the owner of the station where the resource was produced/deposited.
    StationOwner,
    /// Credit is split between owner and deliverer (use goalAttributionOwnerShare for ratio).
    Split
}

/// Who can use a station (interact, consume, produce).
public enum StationUsePolicy
{
    OwnerOnly,
    SameTeam,
    Anyone,
    /// Anyone can use but may require a fee (fee logic is game-specific; policy only signals intent).
    AnyoneWithFee
}

/// Whether other players can see a player's or team's resources/capital.
public enum ResourceVisibility
{
    Public,
    Private
}

/// Scriptable policy configuration: "the current law" for ownership and economy.
/// Used by PolicyManager. Taxation is left out of this base implementation.
[CreateAssetMenu(fileName = "New Policy", menuName = "Simulated Assemblies/Policy Data")]
public class PolicyDataSO : ScriptableObject
{
    [Header("Ownership Model")]
    [Tooltip("How property is understood: communal, per-player, per-team, or hybrid.")]
    public OwnershipModel ownershipModel = OwnershipModel.PrivateIndividual;

    [Header("Stealing / Taking")]
    [Tooltip("Whether a player can take a resource owned by another.")]
    public StealingPolicy stealingPolicy = StealingPolicy.Disallowed;

    [Header("Goal Attribution")]
    [Tooltip("Who gets credit when a goal is completed (e.g., resource delivered).")]
    public GoalAttribution goalAttribution = GoalAttribution.ResourceOwner;
    [Tooltip("When goalAttribution is Split, share of credit for the resource owner (0–1). Rest goes to deliverer.")]
    [Range(0f, 1f)]
    public float goalAttributionOwnerShare = 0.7f;

    [Header("Station Use")]
    [Tooltip("Who can use a station (interact, consume, produce).")]
    public StationUsePolicy stationUsePolicy = StationUsePolicy.Anyone;

    [Header("Sharing / Gifting")]
    [Tooltip("Whether players can voluntarily transfer resources or capital to another.")]
    public bool sharingAllowed = true;

    [Header("Resource Visibility")]
    [Tooltip("Whether other players can see a player's resources/capital.")]
    public ResourceVisibility resourceVisibility = ResourceVisibility.Public;
}
