using System;

[Serializable]
public struct Health {

    public bool alive;
    public int maxHealth;
    public int health;
    public uint wounds;

    public Health(int health, uint wounds = 0) {
        this.alive = true;
        this.maxHealth = health;
        this.health = health;
        this.wounds = wounds;
    }

}
