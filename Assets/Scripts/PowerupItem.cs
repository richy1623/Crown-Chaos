using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupUIItem
{
    private string powerupName;
    private int powerupDuration;
    private Sprite powerupSprite;

    public PowerupUIItem(string powerupName, int powerupDuration, Sprite powerupSprite)
    {
        this.powerupName = powerupName;
        this.powerupDuration = powerupDuration;
        this.powerupSprite = powerupSprite;
    }

    public string getPowerupName()
    {
        return this.powerupName;
    }

    public int getPowerupDuration()
    {
        return this.powerupDuration;
    }

    public Sprite getPowerupSprite()
    {
        return this.powerupSprite;
    }

}
