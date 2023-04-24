using Vision.Models;

namespace Vision.Helpers
{
    public static class DataSyncedEventType
    {
        private static Dictionary<string, Type> types = new Dictionary<string, Type>()
        {
            { "Setting", typeof(Setting) },
            { "Project", typeof(Project) },
            { "WorkflowStep", typeof(WorkflowStep) },
            { "Phase", typeof(Phase) },
            { "Company", typeof(Company) }
        };

        public static string GetEventTypeName(object type)
        {
            var typePair = types.FirstOrDefault(x => x.Value == type.GetType());

            if (typePair.Key == null)
            {
               
            }

            return typePair.Key;
        }

        public static Type GetType(string typeName)
        {
            return types[typeName];
        }
    }
}
