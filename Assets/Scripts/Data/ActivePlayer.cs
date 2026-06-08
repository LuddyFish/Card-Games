public static class ActivePlayer
{
    public static int Id { get; private set; }
    public static string Name { get; private set; }

    public static Settings PlayerSettings { get; private set; }

    public static void SetSettings(Settings data)
    {
        PlayerSettings = data;
    }

    public static void SetId(int id)
    {
        Id = id;
    }

    public static void SetName(string name)
    {
        Name = name;
    }
}
