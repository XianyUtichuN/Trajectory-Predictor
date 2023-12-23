using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour {

    //  ----------引用----------
    private TrajectoryPredictor trajectoryPredictor;            //  抛物线预测

    [SerializeField] private Rigidbody objectToThrow;           //  投出去的物体  一般为刚体
    [SerializeField, Range(0.0f, 50.0f)] private float force;   //  投出去的力量
    [SerializeField] private Transform StartPosition;           //  投出去的位置

    public InputAction fire;

    private void OnEnable() {
        //  获取 trajectoryPredictor 组件
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();

        //  如果没有特定的开始投掷位置 则寻找当前位置
        if (StartPosition == null)
            StartPosition = transform;

        //  鼠标左键发射
        fire.Enable();
        fire.performed += ThrowObject;
    }

    private void Update() {
        Predict();
    }

    private void Predict() {
        trajectoryPredictor.PredictTrajectory(ProjectileData());
    }

    private ProjectileProperties ProjectileData() {
        ProjectileProperties properties = new ProjectileProperties();
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();

        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = force;
        properties.mass = r.mass;
        properties.drag = r.drag;

        return properties;
    }

    private void ThrowObject(InputAction.CallbackContext ctx) {
        Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        thrownObject.AddForce(StartPosition.forward * force, ForceMode.Impulse);
    }
}