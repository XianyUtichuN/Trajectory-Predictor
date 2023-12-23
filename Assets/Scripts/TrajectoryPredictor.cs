using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour {

    #region Members

    private LineRenderer trajectoryLine;
    [SerializeField, Tooltip("标记将显示射弹将击中的位置")] private Transform hitMarker;
    [SerializeField, Range(10, 100), Tooltip(" LineRenderer 可以拥有的最大点数")] private int maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f), Tooltip("用于计算轨迹的时间增量")] private float increment = 0.025f;
    [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")] private float rayOverlap = 1.1f;

    #endregion Members

    private void Start() {
        //  获得 trajectoryLine 的引用
        if (trajectoryLine == null)
            trajectoryLine = GetComponent<LineRenderer>();

        //  使 trajectoryLine 和 hitMarker 可见
        SetTrajectoryVisible(true);
    }

    public void PredictTrajectory(ProjectileProperties projectile) {
        Vector3 velocity = projectile.direction * (projectile.initialSpeed / projectile.mass);  //  速度 = 方向 * 发射力度 / 物体质量
        Vector3 position = projectile.initialPosition;                                          //
        Vector3 nextPosition;                                                                   //  
        float overlap;                                                                          //

        UpdateLineRender(maxPoints, (0, position));                                             //  设置 LineRenderer 起始点

        for (int i = 1; i < maxPoints; i++) {
            //  估计速度并更新下一个预测位置
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);              //  时间增量： 用于计算新速度的间隔，例如间隔0.025s计算一次
            nextPosition = position + velocity * increment;

            //  简单来说：两点之间的距离为 一个单位长度 我将这个单位长度稍微延长 【确保所有东西都能被检测到】
            overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            //  击中表面时停止
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap)) {
                UpdateLineRender(i, (i - 1, hit.point));
                MoveHitMarker(hit);
                break;
            }

            //  If nothing is hit, continue rendering the arc without a visual marker
            hitMarker.gameObject.SetActive(false);
            position = nextPosition;
            UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
        }
    }

    /// <summary>
    /// 允许我们设置 LineRenderer
    /// </summary>
    /// <param name="count">Number of points in our line</param>
    /// <param name="pointPos">The position of an induvidual point</param>
    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos) {
        trajectoryLine.positionCount = count;
        trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment) {
        velocity += Physics.gravity * increment;            //  增加下落速度
        velocity *= Mathf.Clamp01(1f - drag * increment);   //  增加空气阻力 乘了一个百分比
        return velocity;
    }

    private void MoveHitMarker(RaycastHit hit) {
        hitMarker.gameObject.SetActive(true);

        // Offset marker from surface
        float offset = 0.025f;
        hitMarker.position = hit.point + hit.normal * offset;
        hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
    }

    public void SetTrajectoryVisible(bool visible) {
        trajectoryLine.enabled = visible;
        hitMarker.gameObject.SetActive(visible);
    }
}