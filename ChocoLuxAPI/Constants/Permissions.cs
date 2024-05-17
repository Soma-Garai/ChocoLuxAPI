namespace ChocoLuxAPI.Constants
{
    //This is where all the permissions/claims are created based on Module
    public static class Permissions<TModule>
    {
        public static class Module
        {
            public static string View => $"{typeof(TModule).Name}.View";
            public static string Create => $"{typeof(TModule).Name}.Create";
            public static string Edit => $"{typeof(TModule).Name}.Edit";
            public static string Delete => $"{typeof(TModule).Name}.Delete";
        }

        public static List<string> GeneratePermissionsForModule(string module)
        {
            var permissions = new List<string>()
        {
            $"{module}.View",
            $"{module}.Create",
            $"{module}.Edit",
            $"{module}.Delete",
        };

            return permissions;
        }
    }
}
