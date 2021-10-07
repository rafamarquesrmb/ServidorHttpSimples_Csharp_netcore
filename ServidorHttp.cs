using System;
using System.Net;
using System.Net.Sockets;

class ServidorHttp
{
    private TcpListener Controlador {get;set;}
    private int Porta {get;set;}
    private int QtdeRequest { get; set; }
    public ServidorHttp(int porta = 8080)
    {
        this.Porta = porta;
        try
        {
             this.Controlador = new TcpListener(IPAddress.Parse("127.0.0.1"),this.Porta);
             this.Controlador.Start();
             Console.WriteLine($"Servidor HTTP está rodando na porta {this.Porta}.");
             Console.WriteLine($"Para acesar, digite no navegador: http://localhost:{this.Porta}.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao iniciar servidor na porta {this.Porta}: \n{e.Message}");
        }
        
    }
}