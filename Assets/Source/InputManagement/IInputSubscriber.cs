namespace Assets.Source.InputManagement
{
    /// <summary>
    /// Interface for type, that can handle input from InputManagers
    /// </summary>
    public interface IInputSubscriber
    {
        /// <summary>
        /// This will get called when InputManager will get input
        /// </summary>
        /// <param name="input">Input information</param>
        void Handle(InputEvent input);
    }
}
