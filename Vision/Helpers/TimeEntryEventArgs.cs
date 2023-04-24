namespace Vision.Helpers
{

    public class TimeEntryEventArgs : EventArgs
    {
        public int BillingCodeId = 0;
        public string Note = "";
        public bool IsActive = false;
        public int AutotaskType = 0;
        public int WorkflowStepId = 0;
        
    }
}
