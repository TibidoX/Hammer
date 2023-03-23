
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Hammer.Common;
using Hammer.Render;
using Hammer.Object;

namespace Hammer
{
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Window : GameWindow
    {
        Vector3 LightPos = new Vector3(0.0f, 0.0f, 1.0f);

        Shader Shader;

        Texture DiffuseStick, SpecularStick;
        Texture DiffuseHead, SpecularHead;

        List<ObjectRender> ObjectRenderList = new List<ObjectRender>();

        double Time;
        int Side = 1;
        const double Degrees = 40;
        


        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private void DefineShader(Shader Shader)
        {
            Shader.SetVector3("light.position", LightPos);
            Shader.SetVector3("light.direction", (0.0f, 0.0f, -1.0f));
            Shader.SetFloat("light.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(12.5f)));
            Shader.SetFloat("light.outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(17.5f)));
            Shader.SetVector3("viewPos", LightPos);

            // light properties
            Shader.SetVector3("light.ambient", (0.2f, 0.2f, 0.2f));
            // we configure the diffuse intensity slightly higher; the right lighting conditions differ with each lighting method and environment.
            // each environment and lighting type requires some tweaking to get the best out of your environment.
            Shader.SetVector3("light.diffuse", (0.8f, 0.8f, 0.8f));
            Shader.SetVector3("light.specular", (1.0f, 1.0f, 1.0f));
            Shader.SetFloat("light.constant", 1.0f);
            Shader.SetFloat("light.linear", 0.09f);
            Shader.SetFloat("light.quadratic", 0.032f);

            // material properties
            Shader.SetFloat("material.shininess", 0.5f);//0.5
            Shader.Use();
        }



        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            Cylinder stick = new Cylinder(0f, 1.0f, 0f, 0.15f, 1f);  //0f, 1.0f, 0f, 0.15f, 1f
            Cylinder head = new Cylinder(0f, 0.5f, 0f, 0.2f, 0.8f);  //0f, 0.5f, 0f, 0.2f, 0.8f

            Shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/lighting.frag");
            DefineShader(Shader);

            DiffuseStick = Texture.LoadFromFile("../../../Resources/stick.jpg");
            SpecularStick = Texture.LoadFromFile("../../../Resources/stick_specular.jpg");

            DiffuseHead = Texture.LoadFromFile("../../../Resources/head.jpg");
            SpecularHead = Texture.LoadFromFile("../../../Resources/head_specular.jpg");

            ObjectRenderList.Add(new ObjectRender(stick.GetAllTogether(), stick.GetIndices(), Shader, DiffuseStick, SpecularStick));
            ObjectRenderList.Add(new ObjectRender(head.GetAllTogether(), head.GetIndices(), Shader, DiffuseHead, SpecularHead));

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Time += 100.0 * e.Time * Side; 
            if (Math.Abs(Time) > Degrees) Side *= -1; //-1

            var RotationMatrixX = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(Time));//Time
            var RotationMatrixY = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(90));//90
           // var RotationMatrixX = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(Time));
            var TranslationMatrix = Matrix4.CreateTranslation(0, -0.3f, 0); //0, -0.3f, 0

            var model = Matrix4.Identity * RotationMatrixX * RotationMatrixY * TranslationMatrix;
            //var model1 = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(90)) *
            //    Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(-Time)) * Matrix4.CreateTranslation(0f, -0.3f, -1f) * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(-45)); ;// *TranslationMatrix;
            var RotationMatrixX1 = Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(90));
            var RotationMatrixZ1 = Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(-Time));
            var RotationMatrixY1 = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(0));

            var TranslationMatrix1 = Matrix4.CreateTranslation(0f, -0.3f, -1f);//0,-0.3,-1

            var model1 = Matrix4.Identity * RotationMatrixX1 * RotationMatrixZ1 * TranslationMatrix1;

            var Obj = ObjectRenderList[0];
            {

                Obj.Bind();
                Obj.ApplyTexture();
                Obj.UpdateShaderModel(model1);
                Obj.ShaderAttribute();
                Obj.Render();
            }
            Obj = ObjectRenderList[1];
            {

                Obj.Bind();
                Obj.ApplyTexture();
                Obj.UpdateShaderModel(model);
                Obj.ShaderAttribute();
                Obj.Render();
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
}

