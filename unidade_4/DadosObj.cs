using CG_Biblioteca;
using System;
using System.Collections.Generic;
using OpenTK;

namespace CG_N4
{
    public class DadosObj
    {
        public List<Ponto4D> Vertices { get; set; }
        public List<Ponto4D> Normais { get; set; }
        public List<Face> Faces { get; set; }
        public DadosMdl DadosMdl { get; set; }

        public DadosObj()
        {
            Vertices = new List<Ponto4D>();
            Normais = new List<Ponto4D>();
            Faces = new List<Face>();
        }
    }

    public class Face
    {
        public List<int> Indices { get; }
        public List<int> Normais { get; }

        public Face(List<int> indices, List<int> normais)
        {
            Indices = indices;
            Normais = normais;
        }
    }

    public class DadosMdl
    {
        public Color Ambient { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }

        public DadosMdl() 
        {
            Ambient = Color.White;
            Diffuse = Color.Black;
            Specular = Color.Brown;
        }

        public DadosMdl(Color ambient, Color diffuse, Color specular)
        {
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
        }
    }
}
