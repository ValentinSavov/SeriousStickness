using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public interface Danger
{
    bool PointIsDangerous(Vector3 argInPoint);
}*/
/*
public interface DamageAcceptor
{
    void acceptDamage(Messaging.DamageMsgArgs argInArgs);
    List<string> groups { get; set; }
}*/

/*
public interface PlayerTarget
{
    bool isInvulnerable { get; set; }
    Vector3 globalObjective { get; set; }
}*/

public interface WeaponControl
{
    bool isReadyToUse { get; set; }
    PlayerTarget prefferedPlayerTarget { get; set; }
    void Shoot();
    void Arm();
    void Disarm();
    void SetMode(string argInMode);
}

public interface SpawnComponent
{
    int getPriority();
    string getName();
    bool checkSpawnConditions(RandomTileGeneratorScript.Tile argInTile);
    bool additionalCheckPassed(int argInRequiredPattern, RandomTileGeneratorScript.Tile argInTile, int argInRelativeLandLvl);
}

public interface ActiveObject
{
    void SetTile(RandomTileGeneratorScript.Tile argInTile);
}

public interface Player
{
    void statsDoDamage(Messaging.DamageMsgArgs args);
}
