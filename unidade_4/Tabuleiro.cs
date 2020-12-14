using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using OpenTK;
using System.Collections.Generic;

namespace gcgcg
{
    class Tabuleiro : ObjetoGeometria
    {
        private char charAtual = '@';
        List<string> ordemPecas = new List<string>() { "torre", "cavalo", "bispo", "rainha", "rei", "bispo", "cavalo", "torre" };

        public Campo[,] CamposTabuleiro = new Campo[8,8];
        
        protected Color Cor { get; }

        public Tabuleiro(Ponto4D pontoCentro, double tamanho, int altura, char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {
            double minX = pontoCentro.X - tamanho;
            double maxX = pontoCentro.X + tamanho;
            double minY = pontoCentro.Y - altura;
            double maxY = pontoCentro.Y + altura;
            double minZ = pontoCentro.Z - tamanho;
            double maxZ = pontoCentro.Z + tamanho;

            PontosAdicionar(new Ponto4D(minX, minY, maxZ)); // [0] 
            PontosAdicionar(new Ponto4D(maxX, minY, maxZ)); // [1] 
            PontosAdicionar(new Ponto4D(maxX, maxY, maxZ)); // [2] 
            PontosAdicionar(new Ponto4D(minX, maxY, maxZ)); // [3] 
            PontosAdicionar(new Ponto4D(minX, minY, minZ)); // [4] 
            PontosAdicionar(new Ponto4D(maxX, minY, minZ)); // [5] 
            PontosAdicionar(new Ponto4D(maxX, maxY, minZ)); // [6] 
            PontosAdicionar(new Ponto4D(minX, maxY, minZ)); // [7] 

            GerarCasas(pontoCentro, tamanho, altura);
            InicializarPecas(tamanho);
        }

        private void GerarCasas(Ponto4D pontoCentro, double tamanho, int altura)
        {
            double tamanhoCasa = tamanho / 8;

            double xBase = pontoCentro.X - (7 * tamanhoCasa);
            double yBase = pontoCentro.Y;
            double zBase = pontoCentro.Z - (7 * tamanhoCasa);

            for (var i = 0; i < 8; i++)
            {
                double zAtual = zBase + (2 * tamanhoCasa * i);
                double xAtual = xBase;

                for (var j = 0; j < 8; j++)
                {
                    bool espacoEhPreto = (j % 2 == 0 && i % 2 == 0) || (j % 2 == 1 && i % 2 == 1);
                    Color corCampo = espacoEhPreto ? Color.Black : Color.White;

                    Ponto4D pontoCentroCasa = new Ponto4D(xAtual, yBase, zAtual);

                    Campo campo = new Campo(pontoCentroCasa, tamanhoCasa, altura, corCampo, Utilitario.charProximo(charAtual), this);

                    CamposTabuleiro[i, j] = campo;
                    FilhoAdicionar(campo);

                    xAtual += tamanhoCasa * 2;
                }
            }
        }

        private void InicializarPecas(double tamanho)
        {
            double distancia = tamanho / 4;
            double posicao = tamanho / 8;

            for (int i = 0; i < ordemPecas.Count; i++)
            {
                double xInicial = -tamanho + posicao;
                double zInicial = tamanho - posicao - distancia * i;
                int linha = ordemPecas.Count - i - 1;

                CamposTabuleiro[linha, 0].Peca = InicializarPeca(ordemPecas[i], xInicial, zInicial, Color.Yellow);
                CamposTabuleiro[linha, 0].Peca.DirecaoPeca = PecaXadrez.Direcao.Frente;
                CamposTabuleiro[linha, 0].Peca.Indice = i * 2;
            }

            for (int i = 0; i < ordemPecas.Count; i++)
            {
                double xInicial = -tamanho + posicao * 3;
                double zInicial = tamanho - posicao - distancia * i;
                int linha = ordemPecas.Count - i - 1;

                CamposTabuleiro[linha, 1].Peca = InicializarPeca("peao", xInicial, zInicial, Color.Yellow);
                CamposTabuleiro[linha, 1].Peca.DirecaoPeca = PecaXadrez.Direcao.Frente;
                CamposTabuleiro[linha, 1].Peca.Indice = 1 + i * 2;
            }


            for (int i = 0; i < ordemPecas.Count; i++)
            {
                double xInicial = tamanho - posicao;
                double zInicial = tamanho - posicao - distancia * i;
                int linha = ordemPecas.Count - i - 1;

                CamposTabuleiro[linha, 7].Peca = InicializarPeca(ordemPecas[i], xInicial, zInicial, Color.Cyan, 180);
                CamposTabuleiro[linha, 7].Peca.DirecaoPeca = PecaXadrez.Direcao.Tras;
                CamposTabuleiro[linha, 7].Peca.Indice = i * 2;
            }

            for (int i = 0; i < ordemPecas.Count; i++)
            {
                double xInicial = tamanho - posicao * 3;
                double zInicial = tamanho - posicao - distancia * i;
                int linha = ordemPecas.Count - i - 1;

                CamposTabuleiro[linha, 6].Peca = InicializarPeca("peao", xInicial, zInicial, Color.Cyan);
                CamposTabuleiro[linha, 6].Peca.DirecaoPeca = PecaXadrez.Direcao.Tras;
                CamposTabuleiro[linha, 6].Peca.Indice = 1 + i * 2;
            }
        }

        private PecaXadrez InicializarPeca(string tipoPeca, double xInicial, double zInicial, Color cor, double rotacao = 0)
        {
            PecaXadrez pecaCriada;

            switch (tipoPeca.ToLower())
            {
                case "peao":
                    pecaCriada = new Peao(xInicial, zInicial, cor, rotacao, Utilitario.charProximo(charAtual), null);
                    break;
                case "torre":
                    pecaCriada = new Torre(xInicial, zInicial, cor, rotacao, Utilitario.charProximo(charAtual), null);
                    break;
                case "cavalo":
                    pecaCriada = new Cavalo(xInicial, zInicial, cor, rotacao, Utilitario.charProximo(charAtual), null);
                    break;
                case "bispo":
                    pecaCriada = new Bispo(xInicial, zInicial, cor, rotacao, Utilitario.charProximo(charAtual), null);
                    break;
                case "rainha":
                    pecaCriada = new Rainha(xInicial, zInicial, cor, rotacao, Utilitario.charProximo(charAtual), null);
                    break;
                case "rei":
                    pecaCriada = new Rei(xInicial, zInicial, cor, rotacao, Utilitario.charProximo(charAtual), null);
                    break;
                default:
                    return null;
            }

            FilhoAdicionar(pecaCriada);
            return pecaCriada;
        }

        protected override void DesenharObjeto()
        {
            GL.Color3(Color.Black);
            GL.Begin(PrimitiveType.Quads);

            // Face da frente
            GL.Normal3(0, 0, 1);
            GL.Vertex3(pontosLista[0].X, pontosLista[0].Y, pontosLista[0].Z);    // PtoA
            GL.Vertex3(pontosLista[1].X, pontosLista[1].Y, pontosLista[1].Z);    // PtoB
            GL.Vertex3(pontosLista[2].X, pontosLista[2].Y, pontosLista[2].Z);    // PtoC
            GL.Vertex3(pontosLista[3].X, pontosLista[3].Y, pontosLista[3].Z);    // PtoD
            // Face do fundo
            GL.Normal3(0, 0, -1);
            GL.Vertex3(pontosLista[4].X, pontosLista[4].Y, pontosLista[4].Z);    // PtoE
            GL.Vertex3(pontosLista[7].X, pontosLista[7].Y, pontosLista[7].Z);    // PtoH
            GL.Vertex3(pontosLista[6].X, pontosLista[6].Y, pontosLista[6].Z);    // PtoG
            GL.Vertex3(pontosLista[5].X, pontosLista[5].Y, pontosLista[5].Z);    // PtoF
            // Face da direita
            GL.Normal3(1, 0, 0);
            GL.Vertex3(pontosLista[1].X, pontosLista[1].Y, pontosLista[1].Z);    // PtoB
            GL.Vertex3(pontosLista[5].X, pontosLista[5].Y, pontosLista[5].Z);    // PtoF
            GL.Vertex3(pontosLista[6].X, pontosLista[6].Y, pontosLista[6].Z);    // PtoG
            GL.Vertex3(pontosLista[2].X, pontosLista[2].Y, pontosLista[2].Z);    // PtoC
            // Face da esquerda
            GL.Normal3(-1, 0, 0);
            GL.Vertex3(pontosLista[0].X, pontosLista[0].Y, pontosLista[0].Z);    // PtoA
            GL.Vertex3(pontosLista[3].X, pontosLista[3].Y, pontosLista[3].Z);    // PtoD
            GL.Vertex3(pontosLista[7].X, pontosLista[7].Y, pontosLista[7].Z);    // PtoH
            GL.Vertex3(pontosLista[4].X, pontosLista[4].Y, pontosLista[4].Z);    // PtoE

            GL.End();
        }
    }
}
