using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public class CustomEffects
    {
        #region Effect Fields
        public  Effect Phong;
        #endregion

        #region Colors
        public Vector4 color_white = new Vector4(1.0f,1.0f,1.0f,1.0f);
        #endregion

        #region Time
        private float elapsedTime;
        #endregion

        #region Constructors

        public CustomEffects() {
            elapsedTime = 0.0f;
        }

        #endregion

        #region Methods

        #region Phong

        #region Initialize
        public void Init_Phong()
        {
            Phong.Parameters["gColorShift"].SetValue(false);
        }
        #endregion

        public void Update_Phong(Matrix world, Matrix view, Matrix projection, Vector3 eyePos)
        {
            Phong.Parameters["gWorld"].SetValue(world);
            Phong.Parameters["gWIT"].SetValue(Matrix.Invert(Matrix.Transpose(world)));
            Phong.Parameters["gWVP"].SetValue(world * view * projection);
            Phong.Parameters["gEyePosW"].SetValue(eyePos);
            Phong.Parameters["gLightVecW"].SetValue(new Vector3(0.0f, 0.0f, -1.0f));
            Phong.CommitChanges();
        }

        #region Phong Special Effects

        public void Set_Specials_Phong(bool useColorShift, bool useBlob, bool useGlow, bool useRotate)
        {
            Phong.Parameters["gColorShift"].SetValue(useColorShift);
            Phong.Parameters["gBlob"].SetValue(useBlob);
            Phong.Parameters["gGlow"].SetValue(useGlow);
            Phong.Parameters["gRotate"].SetValue(useRotate);
            Phong.CommitChanges();
        }
        public void Update_Time(GameTime time)
        {
            elapsedTime += (float)time.ElapsedGameTime.Milliseconds / 100;
            Phong.Parameters["gTime"].SetValue(elapsedTime);
            Phong.CommitChanges();
        }
        public void Update_Glow(float glowSize,float glowIntensity)
        {
            Phong.Parameters["gInflation"].SetValue(glowSize);
            Phong.Parameters["gGlowExp"].SetValue(glowIntensity);
            Phong.CommitChanges();
        }
        public void Update_Rotate(int axis)
        {
            Phong.Parameters["gRotAxis"].SetValue(axis);
            Phong.CommitChanges();
        }
        #endregion

        #region Set Phong Lighting
        public void Set_Phong_Diffuse(Vector3 diffuseMtrl, Vector4 diffuseLight)
        {
            Phong.Parameters["gDiffuseMtrl"].SetValue(new Vector4(diffuseMtrl,1.0f));
            Phong.Parameters["gDiffuseLight"].SetValue(diffuseLight);
        }
        public void Set_Phong_Ambient(Vector4 ambientMtrl, Vector4 ambientLight)
        {
            Phong.Parameters["gAmbientMtrl"].SetValue(ambientMtrl);
            Phong.Parameters["gAmbientLight"].SetValue(ambientLight);
        }
        public void Set_Phong_Specular(Vector4 specularMtrl, Vector4 specularLight, float specularPower)
        {
            Phong.Parameters["gSpecularMtrl"].SetValue(specularMtrl);
            Phong.Parameters["gSpecularLight"].SetValue(specularLight);
            Phong.Parameters["gSpecularPower"].SetValue(specularPower);
        }
        #endregion

        #endregion

        #endregion
    }


    #region Vertex Declarations

    public struct VertexPositionNormalTextureTangentBinormal
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector3 Tangent;
        public Vector3 Binormal;

        public static readonly VertexElement[] VertexElements =
        new VertexElement[]
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
            new VertexElement(0, sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Tangent, 0),
            new VertexElement(0, sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Binormal, 0),
        };


        public VertexPositionNormalTextureTangentBinormal(Vector3 position, Vector3 normal, Vector2 textureCoordinate, Vector3 tangent, Vector3 binormal)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Tangent = tangent;
            Binormal = binormal;
        }

        public static int SizeInBytes { get { return sizeof(float) * 14; } }

        #endregion
    }
}
