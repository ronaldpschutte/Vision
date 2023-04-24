namespace Vision.Helpers
{
    public class ObjectChangedEventArgs : EventArgs
    {
        
        public string ChangeType { get; set; }
        public int ChangeKey { get; set; } 
        public Guid PersonalGuid { get; set; }   

        public ObjectChangedEventArgs(string type, int key, Guid myGuid )
        {
            ChangeType = type;
            ChangeKey = key;
            PersonalGuid = myGuid;
        }
        // Command : moet nog in enum  Create, Update, Delete
    }
}
