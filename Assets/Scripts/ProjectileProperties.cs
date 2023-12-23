using UnityEngine;

public struct ProjectileProperties
{
    public Vector3 direction;           //  抛射物方向
    public Vector3 initialPosition;     //  抛射物初始位置
    public float initialSpeed;          //  抛射物发射力度
    public float mass;                  //  刚体质量
    public float drag;                  //  刚体空气阻力
}