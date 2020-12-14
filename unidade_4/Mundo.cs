#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private float fovy, aspect, near, far;
        private Vector3 eye, at, up;
        private int displayLista;
        private byte clones = 1;
        private bool comDisplayList = true;

        private static Mundo instanciaMundo = null;

        private Campo CampoSelecionado = null;
        private Campo CampoSendoMovido = null;

        private Mundo(int width, int height) : base(width, height) { }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        private CameraPerspective camera = new CameraPerspective();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;
        private char objetoId = '@';
        private String menuSelecao = "";
        private char menuEixoSelecao = 'z';
        private float deslocamento = 0;
        private bool bBoxDesenhar = false;
        private Tabuleiro Tabuleiro;
        private double TamanhoTabuleiro = 8;


        private bool VezDasBrancas = true;
        List<Campo> PossibilidadesMovimento;
        private bool MovendoPeca = false;

        private float cameraX = 18, cameraY = 18, cameraZ = 18;
        private bool LiberouCamera = false;

        Vector3 olhoCamera = new Vector3(15, 15, 30);
        Vector3 upCamera = new Vector3(0, 1, 0);
#if CG_Privado
    private Cilindro obj_Cilindro;
    private Esfera obj_Esfera;
    private Cone obj_Cone;
#endif
        private Cubo obj_Cubo;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            Tabuleiro = new Tabuleiro(new Ponto4D(0, -2, 0), TamanhoTabuleiro, 2, 'a', null);
            objetosLista.Add(Tabuleiro);

            camera.Eye = new Vector3(cameraX, cameraY, cameraZ);
            camera.Fovy = 1;

#if CG_Privado  //FIXME: arrumar os outros objetos
              objetoId = Utilitario.charProximo(objetoId);
              obj_Cilindro = new Cilindro(objetoId, null);
              obj_Cilindro.ObjetoCor.CorR = 177; obj_Cilindro.ObjetoCor.CorG = 166; obj_Cilindro.ObjetoCor.CorB = 136;
              objetosLista.Add(obj_Cilindro);
              obj_Cilindro.Translacao(2, 'x');

              objetoId = Utilitario.charProximo(objetoId);
              obj_Esfera = new Esfera(objetoId, null);
              obj_Esfera.ObjetoCor.CorR = 177; obj_Esfera.ObjetoCor.CorG = 166; obj_Esfera.ObjetoCor.CorB = 136;
              objetosLista.Add(obj_Esfera);
              obj_Esfera.Translacao(4, 'x');

              objetoId = Utilitario.charProximo(objetoId);
              obj_Cone = new Cone(objetoId, null);
              obj_Cone.ObjetoCor.CorR = 177; obj_Cone.ObjetoCor.CorG = 166; obj_Cone.ObjetoCor.CorB = 136;
              objetosLista.Add(obj_Cone);
              obj_Cone.Translacao(6, 'x');
#endif

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 15, 15, 0 });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1f, 1f, 1f });
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.Fovy, Width / (float)Height, camera.Near, camera.Far);
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, near, far);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 modelview = Matrix4.LookAt(camera.Eye, camera.At, camera.Up);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
#if CG_Gizmo
            Sru3D();
#endif


            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
            if (bBoxDesenhar && (objetoSelecionado != null))
                objetoSelecionado.BBox.Desenhar();

            this.SwapBuffers();
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.H:
                    Utilitario.AjudaTeclado();
                    break;
                case Key.Right:
                    if (MovendoPeca)
                        DestacarProximaJogada(true);
                    else
                        DestacarProximoCampo(true);
                    break;
                case Key.Left:
                    if (MovendoPeca)
                        DestacarProximaJogada(false);
                    else
                        DestacarProximoCampo(false);
                    break;
                case Key.Enter:
                    if (CampoSelecionado != null)
                    {
                        if (MovendoPeca)
                            MoverPeca();
                        else
                            MostrarPossibilidades();
                    }
                    break;
                case Key.Escape:
                    if (MovendoPeca)
                        LimparSelecao();
                    break;
                case Key.K:
                    if (!LiberouCamera)
                        break;
                    cameraX = cameraX + 10;
                    camera.Eye = new Vector3(cameraX, cameraY, cameraZ);
                    break;
                case Key.J:
                    if (!LiberouCamera)
                        break;
                    cameraX = cameraX - 10;
                    camera.Eye = new Vector3(cameraX, cameraY, cameraZ);
                    break;
                case Key.U:
                    if (!LiberouCamera)
                        break;
                    cameraY = cameraY + 10;
                    camera.Eye = new Vector3(cameraX, cameraY, cameraZ);
                    break;
                case Key.I:
                    if (!LiberouCamera)
                        break;
                    cameraY = cameraY - 10;
                    camera.Eye = new Vector3(cameraX, cameraY, cameraZ);
                    break;
                case Key.C:
                    LiberouCamera = !LiberouCamera;
                    break;
            }
        }

        private void DestacarProximoCampo(bool praDireita)
        {
            int indexAtual = CampoSelecionado == null ? -1 : CampoSelecionado.Peca.Indice;

            if (indexAtual <= MenorIndexAtivo() && !praDireita)
                indexAtual = 16;
            else if (indexAtual >= MaiorIndexAtivo() && praDireita)
                indexAtual = -1;

            int proximoIndex = -1;
            if (praDireita)
                proximoIndex = 20;

            Campo campoParaSelecionar = null;

            foreach (Campo campo in Tabuleiro.CamposTabuleiro)
            {
                if (campo.Cor != campo.CorOriginal)
                    campo.Cor = campo.CorOriginal;

                if (campo.Peca == null)
                    continue;

                Color corPecaVez = VezDasBrancas ? Color.Cyan : Color.Yellow;
                if (campo.Peca.Cor != corPecaVez)
                    continue;

                bool pradireitaEIndexMaior = praDireita && campo.Peca.Indice > indexAtual && campo.Peca.Indice < proximoIndex;
                bool praEsquerdaEIndexMenor = !praDireita && campo.Peca.Indice < indexAtual && campo.Peca.Indice > proximoIndex;

                if (pradireitaEIndexMaior || praEsquerdaEIndexMenor)
                {
                    proximoIndex = campo.Peca.Indice;
                    campoParaSelecionar = campo;
                }
            }

            CampoSelecionado = campoParaSelecionar;
            campoParaSelecionar.Cor = Color.Blue;
        }

        private int MenorIndexAtivo()
        {
            int menorIndice = 20;

            foreach (Campo campo in Tabuleiro.CamposTabuleiro)
            {
                if (campo.Cor != campo.CorOriginal)
                    campo.Cor = campo.CorOriginal;

                if (campo.Peca == null)
                    continue;

                Color corPecaVez = VezDasBrancas ? Color.Cyan : Color.Yellow;
                if (campo.Peca.Cor != corPecaVez)
                    continue;
;
                if (campo.Peca.Indice < menorIndice)
                    menorIndice = campo.Peca.Indice;
            }

            return menorIndice;
        }

        private int MaiorIndexAtivo()
        {
            int maiorIndice = -1;

            foreach (Campo campo in Tabuleiro.CamposTabuleiro)
            {
                if (campo.Cor != campo.CorOriginal)
                    campo.Cor = campo.CorOriginal;

                if (campo.Peca == null)
                    continue;

                Color corPecaVez = VezDasBrancas ? Color.Cyan : Color.Yellow;
                if (campo.Peca.Cor != corPecaVez)
                    continue;
                
                if (campo.Peca.Indice > maiorIndice)
                    maiorIndice = campo.Peca.Indice;
            }

            return maiorIndice;
        }

        private void MostrarPossibilidades()
        {
            PossibilidadesMovimento = CampoSelecionado.Peca.MovimentosPossiveis(Tabuleiro.CamposTabuleiro);
            foreach (Campo campo in PossibilidadesMovimento)
                campo.Cor = Color.Red;

            if (PossibilidadesMovimento.Count > 0)
            {
                MovendoPeca = true;
                CampoSendoMovido = CampoSelecionado;
                CampoSendoMovido.Cor = CampoSendoMovido.CorOriginal;
                CampoSelecionado = null;
            }
        }

        private void DestacarProximaJogada(bool praDireita)
        {
            if (CampoSelecionado == null)
            {
                CampoSelecionado = PossibilidadesMovimento[0];
                CampoSelecionado.Cor = Color.Blue;
                return;
            }

            int indexAtual = PossibilidadesMovimento.IndexOf(CampoSelecionado);
            if (praDireita)
                indexAtual++;
            else
                indexAtual--;

            if (indexAtual < 0)
                indexAtual = PossibilidadesMovimento.Count - 1;
            if (indexAtual >= PossibilidadesMovimento.Count)
                indexAtual = 0;

            CampoSelecionado.Cor = Color.Red;
            CampoSelecionado = PossibilidadesMovimento[indexAtual];
            CampoSelecionado.Cor = Color.Blue;
        }

        private void MoverPeca()
        {
            CampoSelecionado.ReceberPeca(CampoSendoMovido.Peca);
            CampoSendoMovido.Peca = null;

            foreach (Campo campo in PossibilidadesMovimento)
                campo.Cor = campo.CorOriginal;

            MovendoPeca = false;
            VezDasBrancas = !VezDasBrancas;

            if (VezDasBrancas)
                camera.Eye = new Vector3(18, 18, 18);
            else
                camera.Eye = new Vector3(-18, 18, 18);

            CampoSelecionado = null;
        }

        private void LimparSelecao()
        {
            foreach (Campo campo in PossibilidadesMovimento)
                campo.Cor = campo.CorOriginal;

            MovendoPeca = false;

            CampoSelecionado = CampoSendoMovido;
            CampoSelecionado.Cor = Color.Blue;
            CampoSendoMovido = null;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
        }

#if CG_Gizmo
        private void Sru3D()
        {
            GL.LineWidth(1);
            GL.Begin(PrimitiveType.Lines);
            // GL.Color3(1.0f,0.0f,0.0f);
            GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
            // GL.Color3(0.0f,1.0f,0.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
            // GL.Color3(0.0f,0.0f,1.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
            GL.End();
        }
#endif
    }

    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(800, 800);
            window.Title = "CG_N4";
            window.Run(1.0 / 60.0);
        }
    }
}
