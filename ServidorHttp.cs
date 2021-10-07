using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class ServidorHttp
{
    private TcpListener Controlador {get;set;}
    private int Porta {get;set;}
    private int QtdeRequest { get; set; }
    private string HtmlExemplo {get;set;}
    public ServidorHttp(int porta = 8080)
    {
        this.Porta = porta;
        this.CriarHtmlExemplo();
        try
        {
            this.Controlador = new TcpListener(IPAddress.Parse("127.0.0.1"),this.Porta);
            this.Controlador.Start();
            Console.WriteLine($"Servidor HTTP está rodando na porta {this.Porta}.");
            Console.WriteLine($"Para acesar, digite no navegador: http://localhost:{this.Porta}.");
            Task servidorHttpTask = Task.Run(() => AguardarRequest());
            servidorHttpTask.GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao iniciar servidor na porta {this.Porta}: \n{e.Message}");
        }
    }
    private async Task AguardarRequest()
    {
        while(true)
        {
            Socket conexao = await this.Controlador.AcceptSocketAsync();
            this.QtdeRequest++;
            Task task = Task.Run(()=> ProcessarRequest(conexao,this.QtdeRequest));
        }       
    }
    private void ProcessarRequest(Socket conexao, int numeroRequest)
    {
        Console.WriteLine($"Processando request #{numeroRequest}...\n");
        if(conexao.Connected)
        {
            byte[] bytesRequisicao = new byte[1024];
            conexao.Receive(bytesRequisicao, bytesRequisicao.Length, 0);
            string textoRequisicao = Encoding.UTF8.GetString(bytesRequisicao)
                .Replace((char)0, ' ').Trim();
            if(textoRequisicao.Length > 0)
            {
                Console.WriteLine($"\n{textoRequisicao}\n");
                var bytesConteudo = LerArquivo("/index.html");
                var bytesCabecalho = GerarCabecalho("HTTP/1.1", "text/html;chaset=utf-8", "200",bytesConteudo.Length);
                int bytesEnviados = conexao.Send(bytesCabecalho, bytesCabecalho.Length, 0);
                bytesEnviados += conexao.Send(bytesConteudo, bytesConteudo.Length, 0);
                conexao.Close();
                Console.WriteLine($"\n{bytesEnviados} bytes enviados em resposta à requisição #{numeroRequest}.");
            }
        }
        Console.WriteLine($"\nRequest {numeroRequest} finalizado.");
    }
    public byte[] GerarCabecalho(string versaoHttp, string tipoMime, string codigoHttp, int qtdeBytes = 0)
    {
        StringBuilder texto = new StringBuilder();
        texto.Append($"{versaoHttp} {codigoHttp}{Environment.NewLine}");
        texto.Append($"Server: Servidor Http Simples 1.0{Environment.NewLine}");
        texto.Append($"Content-Type: {tipoMime}{Environment.NewLine}");
        texto.Append($"Content-Length: {qtdeBytes}{Environment.NewLine}{Environment.NewLine}");
        return Encoding.UTF8.GetBytes(texto.ToString());

    }
    private void CriarHtmlExemplo()
    {
        StringBuilder html = new StringBuilder();
        html.Append("<!DOCTYPE html><html lang =\"pt-br\"><head><meta charset=\"UTF-8\">");
        html.Append("<meta name=\"viewport\" content=\"width=device=width, initial-scale=1.0\">");
        html.Append("<title>Página Estática</title></head><body>");
        html.Append("<h1>Página Estática</h1></body></html>");
        this.HtmlExemplo = html.ToString();
    }
    public byte[] LerArquivo(string recurso)
    {
        string diretorio = "D:\\dev\\curso_ricardo_maroquio\\ServidorHttpSimples\\www";
        string caminhoArquivo = diretorio + recurso.Replace("/", "\\");
        if(File.Exists(caminhoArquivo))
        {
            return File.ReadAllBytes(caminhoArquivo);
        }
        else return new byte[0];
    }
}