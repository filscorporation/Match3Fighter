namespace MatchServer
{
    /// <summary>
    /// Info about player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// ID of client connection
        /// </summary>
        public int ClientID;

        /// <summary>
        /// Players device unique identifier
        /// </summary>
        public string PlayerID;

        public Player(int clientID)
        {
            ClientID = clientID;
        }
    }
}
