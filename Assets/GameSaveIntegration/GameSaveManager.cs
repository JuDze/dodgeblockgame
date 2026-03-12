using Game.SaveIntegration.Data;
using Game.SaveIntegration.Validation;
using SaveSystem.Core;
using SaveSystem.Versioning;

public static class GameSaveManager
{
    private static SaveManager<GameSaveData> _manager;
    public static bool LastLoadWasCorrupted { get; private set; }

    private static SaveManager<GameSaveData> Get()
    {
        if (_manager == null)
        {
            _manager = SaveManagerFactory.Create(
                defaultData:        new GameSaveData(),
                validator:          new GameSaveDataValidator(),
                registerMigrations: m => { },
                getVersion:         d => d.saveVersion,
                setVersion:         (d, v) => d.saveVersion = v,
                currentVersion:     1,
                masterSecret:       "DodgeBlocks_juDze_X7k#mP2q",
                fileName:           "gamesave.json"
            );
        }

        return _manager;
    }

    public static void Save(GameSaveData data)
    {
        LastLoadWasCorrupted = false;
        Get().Save(data);
    }

    public static void Delete()
    {
        Get().DeleteSave();
        _manager = null;
        LastLoadWasCorrupted = false;
    }

    public static bool HasSave()
    {
        return Get().HasSave();
    }

    public static GameSaveData Load()
    {
        LastLoadWasCorrupted = false;
        return Get().Load();
    }
}