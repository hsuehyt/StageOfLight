using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionWallLeft : MonoBehaviour
{
    public Camera Acamera;
    public GameObject AScreenOBJ;
    public GameObject UserOBJ;

    public Vector3 userheadLimit;

    [Tooltip("Width of the display in meters.")]
    public float screenWidth = 1.6f; // 0.88f;

    [Tooltip("Height of the display in meters.")]
    public float screenHeight = 0.9f; // 0.50f;

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
        Acamera = GetComponent<Camera>();

        initialCamPos = Acamera.transform.position;
        initialCamRot = Acamera.transform.rotation;
        initialCamMat = Matrix4x4.TRS(initialCamPos, initialCamRot, Vector3.one);
        initialCamPrjMat = Acamera.projectionMatrix;

    }

    void LateUpdate()
    {
        userheadLimit = UserOBJ.transform.position;

        standardUserDistance = Mathf.Abs(AScreenOBJ.transform.position.x - userheadLimit.x);
        left = Acamera.nearClipPlane * (-screenWidth / 2 - userheadLimit.z + AScreenOBJ.transform.position.z) / standardUserDistance; // initialRelPos.z;
        right = Acamera.nearClipPlane * (screenWidth / 2 - userheadLimit.z + AScreenOBJ.transform.position.z) / standardUserDistance; // initialRelPos.z;


        bottom = Acamera.nearClipPlane * (-screenHeight / 2 - userheadLimit.y + AScreenOBJ.transform.position.y) / standardUserDistance; // initialRelPos.z;
        top = Acamera.nearClipPlane * (screenHeight / 2 - userheadLimit.y + AScreenOBJ.transform.position.y) / standardUserDistance; // initialRelPos.z;


        Vector3 headCamPos = new Vector3(userheadLimit.x, userheadLimit.y, userheadLimit.z);
        Quaternion headCamRot = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        Matrix4x4 camPoseMat = Matrix4x4.TRS(headCamPos, headCamRot, Vector3.one);
        camPoseMat = camPoseMat * initialCamMat;

        Acamera.transform.position = camPoseMat.GetColumn(3);
        Acamera.transform.localRotation = camPoseMat.rotation;



        Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, Acamera.nearClipPlane, Acamera.farClipPlane);

        Acamera.projectionMatrix = m;

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
