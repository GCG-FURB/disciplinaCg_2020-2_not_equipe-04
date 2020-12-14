using System.Collections.Generic;
using System.IO;
using CG_Biblioteca;
using OpenTK;

namespace CG_N4
{
    public class LeitorObj
    {
        public static DadosObj LerArquivoObj(string nomeArquivo)
        {
            DadosObj dados = new DadosObj();
            
            string arquivo = $@"../Imagens/{nomeArquivo}.obj";
            string conteudoArquivo = File.ReadAllText(arquivo);

            // Observação: Nos nossos arquivos, as imagens não tem texturas, por isso não é necessário verificá-las
            // Nós vamos adicionar apenas uma cor no final, identificando se a peça é branca ou preta

            string arquivoMtllib = "";

            foreach (string linha in conteudoArquivo.Split("\n"))
            {
                string[] valores = linha.Split(" ");

                double x = 0;
                double y = 0;
                double z = 0;

                switch (valores[0])
                {
                    case "v":
                        x = (double.Parse(valores[1]) / 1000000);
                        y = (double.Parse(valores[2]) / 1000000);
                        z = (double.Parse(valores[3]) / 1000000);

                        dados.Vertices.Add(new Ponto4D(x, y, z));
                        break;
                    case "vn":
                        x = double.Parse(valores[1]);
                        y = double.Parse(valores[2]);
                        z = double.Parse(valores[3]);

                        dados.Normais.Add(new Ponto4D(x, y, z));
                        break;
                    case "f":
                        List<int> indices = new List<int>();
                        List<int> normais = new List<int>();

                        for (int i = 1; i < valores.Length; i++)
                        {
                            string[] valoresFace = valores[i].Split("/");
                            
                            indices.Add(int.Parse(valoresFace[0]));
                            normais.Add(int.Parse(valoresFace[2]));
                        }

                        dados.Faces.Add(new Face(indices, normais));
                        break;
                    case "mtllib":
                        arquivoMtllib = valores[1];
                        break;
                }
            }

            if (!string.IsNullOrEmpty(arquivoMtllib))
            {
                dados.DadosMdl = new DadosMdl();

                arquivo = $@"../Imagens/{arquivoMtllib}";
                conteudoArquivo = File.ReadAllText(arquivo);

                foreach (string linha in conteudoArquivo.Split("\n"))
                {
                    string[] valores = linha.Split(" ");

                    int r = 0;
                    int g = 0;
                    int b = 0;

                    switch (valores[0])
                    {
                        case "Ka":
                            r = (int)((float.Parse(valores[1]) / 1000000) * 255);
                            g = (int)((float.Parse(valores[2]) / 1000000) * 255);
                            b = (int)((float.Parse(valores[3]) / 1000000) * 255);

                            dados.DadosMdl.Ambient = Color.FromArgb(255, r, g, b);
                            break;
                        case "Kd":
                            r = (int)((float.Parse(valores[1]) / 1000000) * 255);
                            g = (int)((float.Parse(valores[2]) / 1000000) * 255);
                            b = (int)((float.Parse(valores[3]) / 1000000) * 255);

                            dados.DadosMdl.Diffuse = Color.FromArgb(255, r, g, b);
                            break;
                        case "Ks":
                            r = (int)((float.Parse(valores[1]) / 1000000) * 255);
                            g = (int)((float.Parse(valores[2]) / 1000000) * 255);
                            b = (int)((float.Parse(valores[3]) / 1000000) * 255);

                            dados.DadosMdl.Specular = Color.FromArgb(255, r, g, b);
                            break;
                    }
                }
            }

            return dados;
        }
    }
}
