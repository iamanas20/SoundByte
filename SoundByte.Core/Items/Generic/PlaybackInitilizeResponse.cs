namespace SoundByte.Core.Items.Generic
{
    public class PlaybackInitilizeResponse
    {
        public PlaybackInitilizeResponse(bool success = true, string messsage = null)
        {
            Success = success;
            Message = messsage;
        }

        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
