using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;
using CG_Biblioteca;

namespace gcgcg
{
    internal abstract class Objeto
    {
        protected char rotulo;
        private Cor objetoCor = new Cor(255, 255, 255, 255);
        public Cor ObjetoCor { get => objetoCor; set => objetoCor = value; }
        private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
        public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
        private float primitivaTamanho = 1;
        public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
        private BBox bBox = new BBox();
        public BBox BBox { get => bBox; set => bBox = value; }
        private List<Objeto> objetosLista = new List<Objeto>();

        private Transformacao4D matriz = new Transformacao4D();
        public Transformacao4D Matriz { get => matriz; }

        /// Matrizes temporarias que sempre sao inicializadas com matriz Identidade entao podem ser "static".
        private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
        private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
        private static Transformacao4D matrizTmpEscala = new Transformacao4D();
        private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
        private static Transformacao4D matrizGlobal = new Transformacao4D();

        private enum menuObjetoEnum { Translacao, Escala, Rotacao }
        private menuObjetoEnum menuObjetoOpcao;

        public Objeto(char rotulo, Objeto paiRef)
        {
            this.rotulo = rotulo;
        }

        public void Desenhar()
        {
            GL.PushMatrix();                                    // N3-Exe14: grafo de cena
            GL.MultMatrix(matriz.ObterDados());
            GL.Color3(objetoCor.CorR, objetoCor.CorG, objetoCor.CorB);
            GL.LineWidth(primitivaTamanho);
            GL.PointSize(primitivaTamanho);
            DesenharGeometria();
            for (var i = 0; i < objetosLista.Count; i++)
            {
                objetosLista[i].Desenhar();
            }
            GL.PopMatrix();                                     // N3-Exe14: grafo de cena
        }
        protected abstract void DesenharGeometria();

        public void FilhoAdicionar(Objeto filho)
        {
            this.objetosLista.Add(filho);
        }

        public void FilhoRemover(Objeto filho)
        {
            this.objetosLista.Remove(filho);
        }

        public void AtribuirIdentidade()
        {
            matriz.AtribuirIdentidade();
        }
        public void Translacao(double x, double y, double z)
        {
            Transformacao4D a = new Transformacao4D();
            a.AtribuirElemento(12, x);
            a.AtribuirElemento(13, y);
            a.AtribuirElemento(14, z);

            matriz = matrizTmpTranslacao.MultiplicarMatriz(a);
        }

        public void Escala(double x, double y, double z)
        {
            Transformacao4D a = new Transformacao4D();
            a.AtribuirElemento(0, x);
            a.AtribuirElemento(5, y);
            a.AtribuirElemento(10, z);

            matriz = matrizTmpTranslacao.MultiplicarMatriz(a);

        }

        public void Rotation(double angulo, double x, double y, double z)
        {
            if (angulo == 0)
                return;

            Transformacao4D a = new Transformacao4D();
            var cosseno = Math.Cos(angulo);
            var seno = Math.Sin(angulo);

            if (x == 0)
            {
                a.AtribuirElemento(5, cosseno);
                a.AtribuirElemento(6, -seno);
                a.AtribuirElemento(9, seno);
                a.AtribuirElemento(10, cosseno);
            }
            else
            if (y == 0)
            {
                a.AtribuirElemento(0, cosseno);
                a.AtribuirElemento(2, seno);
                a.AtribuirElemento(8, -seno);
                a.AtribuirElemento(10, cosseno);
            }
            else
            if (z == 0)
            {
                a.AtribuirElemento(0, cosseno);
                a.AtribuirElemento(1, -seno);
                a.AtribuirElemento(4, seno);
                a.AtribuirElemento(5, cosseno);
            }
            if (x != 0)
                a.AtribuirElemento(12, x);
            if (y != 0)
                a.AtribuirElemento(13, y);
            if (z != 0)
                a.AtribuirElemento(14, z);

            matriz = matrizTmpTranslacao.MultiplicarMatriz(a);
        }

        public void MenuTecla(OpenTK.Input.Key tecla, char eixo, float deslocamento, bool bBox)
        {
            if (tecla == Key.P)
            {
                Console.WriteLine(this);
                if (bBox)
                {
                    Console.Write(BBox);
                }
            }
            else if (tecla == Key.M) Console.WriteLine(this.Matriz);
            else if (tecla == Key.R)
            {
                this.AtribuirIdentidade();
            }
            else if (tecla == Key.Up) menuObjetoOpcao++;
            else if (tecla == Key.Down) menuObjetoOpcao--; //TODO: qdo chega indice 0 não vai para o final

            if (!Enum.IsDefined(typeof(menuObjetoEnum), menuObjetoOpcao))
                menuObjetoOpcao = menuObjetoEnum.Translacao;

            Console.WriteLine("__ Objeto (" + menuObjetoOpcao + "," + eixo + "," + deslocamento + ")");
            if ((tecla == Key.Left) || (tecla == Key.Right))
            {
                switch (menuObjetoOpcao)
                {
                    case menuObjetoEnum.Translacao:
                        if (tecla == Key.Left)
                            deslocamento = -deslocamento;
                        //this.Translacao(deslocamento, eixo);
                        break;
                    case menuObjetoEnum.Escala:
                        if (deslocamento > 1)
                        {
                            if (tecla == Key.Left)
                                deslocamento = 1 / deslocamento;
                            ///  this.Escala(deslocamento);
                        }
                        break;
                    case menuObjetoEnum.Rotacao:
                        if (tecla == Key.Left)
                            deslocamento = -deslocamento;
                        //this.Rotacao(deslocamento, eixo, bBox); //TODO: deslocamento (float) .. angulo (double)
                        break;
                }
            }
        }

    }
}