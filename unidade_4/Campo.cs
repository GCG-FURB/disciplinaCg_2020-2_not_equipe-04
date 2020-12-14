using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace gcgcg
{
    internal class Campo : ObjetoGeometria
    {
        public PecaXadrez Peca { get; set; }
        public Color Cor { get; set; }
        public Color CorOriginal { get; private set; }
        public Ponto4D PontoCentral { get; private set; }

        public Campo(Ponto4D pontoCentro, double tamanho, int altura, Color cor, char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
            Cor = cor;
            CorOriginal = cor;
            PontoCentral = pontoCentro;

            double minX = pontoCentro.X - tamanho;
            double maxX = pontoCentro.X + tamanho;
            double minY = pontoCentro.Y - altura;
            double maxY = pontoCentro.Y + altura;
            double minZ = pontoCentro.Z - tamanho;
            double maxZ = pontoCentro.Z + tamanho;

            PontosAdicionar(new Ponto4D(minX, minY, minZ)); // [0]
            PontosAdicionar(new Ponto4D(maxX, minY, minZ)); // [1]
            PontosAdicionar(new Ponto4D(maxX, minY, maxZ)); // [2]
            PontosAdicionar(new Ponto4D(minX, minY, maxZ)); // [3]
            PontosAdicionar(new Ponto4D(minX, maxY, maxZ)); // [4]
            PontosAdicionar(new Ponto4D(maxX, maxY, maxZ)); // [5]
            PontosAdicionar(new Ponto4D(maxX, maxY, minZ)); // [6]
            PontosAdicionar(new Ponto4D(minX, maxY, minZ)); // [7]

            PrimitivaTipo = PrimitiveType.Quads;
        }

        protected override void DesenharObjeto()
        {
            GL.Color3(Cor);

            GL.Begin(PrimitiveType.Quads);

            GL.Normal3(0, -1, 0); // Face de baixo
            GL.Vertex3(base.pontosLista[0].X, base.pontosLista[0].Y, base.pontosLista[0].Z);
            GL.Vertex3(base.pontosLista[1].X, base.pontosLista[1].Y, base.pontosLista[1].Z);
            GL.Vertex3(base.pontosLista[2].X, base.pontosLista[2].Y, base.pontosLista[2].Z);
            GL.Vertex3(base.pontosLista[3].X, base.pontosLista[3].Y, base.pontosLista[3].Z);

            GL.Normal3(0, 1, 0); // Face de cima 
            GL.Vertex3(base.pontosLista[4].X, base.pontosLista[4].Y, base.pontosLista[4].Z);
            GL.Vertex3(base.pontosLista[5].X, base.pontosLista[5].Y, base.pontosLista[5].Z);
            GL.Vertex3(base.pontosLista[6].X, base.pontosLista[6].Y, base.pontosLista[6].Z);
            GL.Vertex3(base.pontosLista[7].X, base.pontosLista[7].Y, base.pontosLista[7].Z);

            GL.End();
        }

        public void ReceberPeca(PecaXadrez novaPeca)
        {
            if (Peca != null)
                Peca.DestruirPeca();

            Peca = novaPeca;
            Peca.Translacao(PontoCentral.X, 0, PontoCentral.Z);
        }
    }
}
