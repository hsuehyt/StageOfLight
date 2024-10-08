using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionGroundSlanted : MonoBehaviour
{
    public Camera Projection; // Renamed from Acamera
    public GameObject Wall; // Renamed from AScreenOBJ
    public GameObject PointOfView; // Renamed from UserOBJ

    public Vector3 OffsetAngle;
    public float BottomOffset;
    public float TopOffset;
    public float DistanceOffset;

    public Vector3 UserHeadLimit; // Renamed from userheadLimit

    [Tooltip("Width of the display in meters.")]
    public float screenWidth = 1.6f;

    [Tooltip("Height of the display in meters.")]
    public float screenHeight = 0.9f;

    private float left = -0.2F;
    private float right = 0.2F;
    private float bottom = -0.2F;
    private float top = 0.2F;

    public float standardUserDistance = 2f;

    private Vector3 initialCamPos = Vector3.zero;
    private Quaternion initialCamRot = Quaternion.identity;
    private Matrix4x4 initialCamMat = Matrix4x4.identity;
    private Matrix4x4 initialCamPrjMat = Matrix4x4.identity;

    private Quaternion initialHeadRot = Quaternion.Euler(0f, 180f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        Projection = GetComponent<Camera>();
        initialCamPos = Projection.transform.position;
        initialCamRot = Projection.transform.rotation;
        initialCamMat = Matrix4x4.TRS(initialCamPos, initialCamRot, Vector3.one);
        initialCamPrjMat = Projection.projectionMatrix;
    }

    void LateUpdate()
    {
        // Get the absolute position of the user's head (PointOfView)
        UserHeadLimit = PointOfView.transform.position;

        // Calculate standard user distance using absolute values
        standardUserDistance = Mathf.Abs(Wall.transform.position.y - UserHeadLimit.y) + DistanceOffset;

        // Calculate the new left, right, bottom, top using absolute values
        left = Projection.nearClipPlane * (-screenWidth / 2 - UserHeadLimit.x + Wall.transform.position.x) / standardUserDistance;
        right = Projection.nearClipPlane * (screenWidth / 2 - UserHeadLimit.x + Wall.transform.position.x) / standardUserDistance;
        bottom = Projection.nearClipPlane * (-screenHeight / 2 + Wall.transform.position.z - UserHeadLimit.z + BottomOffset) / standardUserDistance;
        top = Projection.nearClipPlane * (screenHeight / 2 + Wall.transform.position.z - UserHeadLimit.z + TopOffset) / standardUserDistance;

        // Set the camera's position to the absolute head position (directly without additional matrix transformations)
        Projection.transform.position = new Vector3(UserHeadLimit.x, UserHeadLimit.y, UserHeadLimit.z);

        // Maintain camera's rotation with the OffsetAngle applied
        Quaternion headCamRot = Quaternion.LookRotation(Vector3.down, Vector3.up);
        Quaternion OffsetRot = Quaternion.Euler(OffsetAngle);
        Projection.transform.rotation = headCamRot * OffsetRot;

        // Update the camera's projection matrix
        Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, Projection.nearClipPlane, Projection.farClipPlane);
        Projection.projectionMatrix = m;
    }

    private Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;

        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;

        return m;
    }
}
