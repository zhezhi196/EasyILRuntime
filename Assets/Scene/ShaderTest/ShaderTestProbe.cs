using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ShaderTestProbe : MonoBehaviour
{
    MeshRenderer render;
    Material mat;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        mat = render.material;
        //CalculateSHVairentMimicUnity(sh);
    }

    private void Update()
    {
        UpdataLightDir();
    }

    private void UpdataLightDir()
    {
        SphericalHarmonicsL2 sh;
        LightProbes.GetInterpolatedProbe(render.probeAnchor ? render.probeAnchor.position : transform.position, render, out sh);
        SHL2ScaleData scaleData = CreateScaleData(sh);
        RawSHl2Data rawData = ScaleData2Raw(scaleData);
        Vector4 lightDir = LightDir(rawData);
        mat.SetVector("_LightDir", lightDir);
    }

    List<Vector4> CalculateSHVairentMimicUnity(SphericalHarmonicsL2 sh)
    {
        List<Vector4> Y = new List<Vector4>();
        for (int ic = 0; ic < 3; ++ic)
        {
            Vector4 coefs = new Vector4();
            coefs.x = sh[ic, 3];
            coefs.y = sh[ic, 1];
            coefs.z = sh[ic, 2];
            coefs.w = sh[ic, 0] - sh[ic, 6];
            Y.Add(coefs);
        }
        for (int ic = 0; ic < 3; ++ic)
        {
            Vector4 coefs = new Vector4();
            coefs.x = sh[ic, 4];
            coefs.y = sh[ic, 5];
            coefs.z = sh[ic, 6] * 3.0f;
            coefs.w = sh[ic, 7];
            Y.Add(coefs);
        }
        {
            Vector4 coefs = new Vector4();
            coefs.x = sh[0, 8];
            coefs.y = sh[1, 8];
            coefs.z = sh[2, 8];
            coefs.w = 1.0f;
            Y.Add(coefs);
        }
        Debug.Log(Y.Count);
        return Y;
    }

    public class SHL2ScaleData
    {
        public Vector3[] scaleSHData = new Vector3[9];
    }

    public class  RawSHl2Data
    {
        public Vector3[] sh9Data = new Vector3[9];
    }

    public SHL2ScaleData CreateScaleData(SphericalHarmonicsL2 sh)
    {
        SHL2ScaleData scaleData = new SHL2ScaleData();
        scaleData.scaleSHData[0] = new Vector3(sh[0, 0], sh[1, 0], sh[2, 0]);
        scaleData.scaleSHData[1] = new Vector3(sh[0, 1], sh[1, 1], sh[2, 1]);
        scaleData.scaleSHData[2] = new Vector3(sh[0, 2], sh[1, 2], sh[2, 2]);
        scaleData.scaleSHData[3] = new Vector3(sh[0, 3], sh[1, 3], sh[2, 3]);
        scaleData.scaleSHData[4] = new Vector3(sh[0, 4], sh[1, 4], sh[2, 4]);
        scaleData.scaleSHData[5] = new Vector3(sh[0, 5], sh[1, 5], sh[2, 5]);
        scaleData.scaleSHData[6] = new Vector3(sh[0, 6], sh[1, 6], sh[2, 6]);
        scaleData.scaleSHData[7] = new Vector3(sh[0, 7], sh[1, 7], sh[2, 7]);
        scaleData.scaleSHData[8] = new Vector3(sh[0, 8], sh[1, 8], sh[2, 8]);
        return scaleData;
    }

    private static readonly float BAND0_FACTOR = 0.5f / Mathf.Sqrt(Mathf.PI);
    private static readonly float BAND1_FACTOR = Mathf.Sqrt(1f/3f/Mathf.PI);
    private static readonly float BAND2_FACTOR_COMMON = 0.124f*Mathf.Sqrt(15f/Mathf.PI);
    private static readonly float BAND2_FACTOR_MO = 0.0625f*Mathf.Sqrt(5f/Mathf.PI);

    public RawSHl2Data ScaleData2Raw(SHL2ScaleData scaleData)
    {
        RawSHl2Data data = new RawSHl2Data();
        //band 0
        //l=0 m=0
        data.sh9Data[0] = scaleData.scaleSHData[0] / BAND0_FACTOR;
        //band 1
        //l=1 m=-1
        data.sh9Data[1] = -scaleData.scaleSHData[1] / BAND1_FACTOR;
        //l=1 m=0
        data.sh9Data[2] = scaleData.scaleSHData[2] / BAND1_FACTOR;
        //l=1 m=1
        data.sh9Data[3] = -scaleData.scaleSHData[3] / BAND1_FACTOR;
        //band2
        //l=2 m=-2
        data.sh9Data[4] = scaleData.scaleSHData[4] / BAND2_FACTOR_COMMON;
        //l=2 m=-1
        data.sh9Data[5] = -scaleData.scaleSHData[5] / BAND2_FACTOR_COMMON;
        //l=2 m=0
        data.sh9Data[6] = scaleData.scaleSHData[6] / BAND2_FACTOR_MO;
        //l=2 m=1
        data.sh9Data[7] = -scaleData.scaleSHData[7] / BAND2_FACTOR_COMMON;
        //l=2 m=2
        data.sh9Data[8] = scaleData.scaleSHData[8] / BAND2_FACTOR_COMMON;
        return data;
    }

    public Vector4 LightDir(RawSHl2Data data)
    {
        //Vector4 max;
        //if (Mathf.Abs(data.sh9Data[1].magnitude) > Mathf.Abs(data.sh9Data[3].magnitude))
        //{
        //    max = data.sh9Data[1];
        //}
        //else {
        //    max = data.sh9Data[3];
        //}
        //if (Mathf.Abs(data.sh9Data[2].magnitude) > Mathf.Abs(max.magnitude))
        //{
        //    max = data.sh9Data[2];
        //}
        //Vector4 dir = max;
        //Vector4 dir = new Vector4(-data.sh9Data[3].magnitude, -data.sh9Data[1].magnitude,
        //   data.sh9Data[2].magnitude, 1);
        Vector4 dir = new Vector4(-data.sh9Data[3].x, -data.sh9Data[1].x,
          data.sh9Data[2].x, 1);
        return dir *(16f*Mathf.PI/17f);
    }

    //private float[,] EvaluateRGB(List<Vector4> src_coefs)
    //{
    //    float[,] coefs = new float[3, 9];
    //    int w = 20;
    //    int h = 20;
    //    float da = 1.0f / ((float)w * (float)h);  //differential of surface area
    //    float addOnW = 1.0f / (float)w * 0.5f;
    //    float addOnH = 1.0f / (float)h * 0.5f;
    //    for (int face = 0; face < 6; ++face)
    //    {
    //        for (int j = 0; j < w; ++j)
    //        {
    //            for (int i = 0; i < h; ++i)
    //            {
    //                float px = (float)i + 0.5f;
    //                float py = (float)j + 0.5f;
    //                float u = 2.0f * ((float)i / (float)w) - 1.0f + addOnW;
    //                float v = 2.0f * ((float)j / (float)h) - 1.0f + addOnH;

    //                var pos = CubeUV2XYZW(u, v, face);
    //                Color col = RebuildColorUnity(pos, src_coefs);  //sample from origin data
    //                col = col * da;
    //                var Y = GetBase(pos);
    //                for (int idx = 0; idx < 9; ++idx)
    //                {
    //                    coefs[0, idx] += Y[idx] * col.r;    //integration
    //                    coefs[1, idx] += Y[idx] * col.g;
    //                    coefs[2, idx] += Y[idx] * col.b;
    //                }
    //            }
    //        }
    //    }

    //    return coefs;
    //}

    static float fc0 = 1.0f / 2.0f * Mathf.Sqrt(1.0f / Mathf.PI);
    static float fc1 = 2.0f / 3.0f * Mathf.Sqrt(3.0f / (4.0f * Mathf.PI));
    static float fc2 = 1.0f / 4.0f * 1.0f / 2.0f * Mathf.Sqrt(15.0f / Mathf.PI);
    static float fc3 = 1.0f / 4.0f * 1.0f / 4.0f * Mathf.Sqrt(5.0f / Mathf.PI);
    static float fc4 = 1.0f / 4.0f * 1.0f / 4.0f * Mathf.Sqrt(15.0f / Mathf.PI);
    List<Vector4> CalculateSHVarientNoUnity(float[,] sh)
    {
        List<Vector4> Y = new List<Vector4>();

        for (int ic = 0; ic < 3; ++ic)
        {
            //each time loops on one color channel
            //eg. deal on R channel -> (coef_1, coef_2, coef_3, coef_0)
            Vector4 coefs = new Vector4();
            coefs.x = fc1 * sh[ic, 1];
            coefs.y = fc1 * sh[ic, 2];
            coefs.z = fc1 * sh[ic, 3];
            coefs.w = fc0 * sh[ic, 0];
            Y.Add(coefs);
        }
        for (int ic = 0; ic < 3; ++ic)
        {
            Vector4 coefs = new Vector4();
            coefs.x = fc2 * sh[ic, 4];
            coefs.y = fc2 * sh[ic, 5];
            coefs.z = fc3 * sh[ic, 6];
            coefs.w = fc2 * sh[ic, 7];
            Y.Add(coefs);
        }
        {
            Vector4 coefs = new Vector4();
            coefs.x = fc4 * sh[0, 8];
            coefs.y = fc4 * sh[1, 8];
            coefs.z = fc4 * sh[2, 8];
            coefs.w = 1.0f;
            Y.Add(coefs);
        }

        return Y;
    }
}
