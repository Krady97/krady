namespace Soulwake.Game.Souls
{
    public interface ISoulInteractable
    {
        bool CanInteract { get; }
        string PromptText { get; }
        void Interact();
    }
}
