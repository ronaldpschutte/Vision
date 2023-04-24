namespace Vision.Models
{
    public class CustomPropertyChangedEventArgs : EventArgs
    {
        public Guid ObjectGuid { get; set; }
        public string Command { get; set; }
        public object? OldObject { get; set; }
        public object? NewObject { get; set; }

        public CustomPropertyChangedEventArgs(Guid objectGuid,string command, object? oldO, object? newO)
        {
            ObjectGuid = objectGuid;
            Command = command;
            OldObject = oldO;
            NewObject = newO;
        }
        // Command : moet nog in enum  Create, Update, Delete
    }
}
