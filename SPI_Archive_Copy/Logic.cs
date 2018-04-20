using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace SPI_Archive_Copy
{
    class Logic
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public bool Ativo = true;


        public void Process()
        {
            try
            {
                Log.Debug("Iniciou Thread");
                //Definindo o tempo de execução
                int timer = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TimeProcess"].ToString());

               Log.Info("Monitorando se existe um arquivo para iniciar a Transferência de arquivo!!!");

                while (Ativo)
                {
                    //Definindo o diretorio de origem
                    string source = ConfigurationManager.AppSettings["Source"].ToString();
                    string sourceFile = @source;

                    //Definindo o diretório de destino
                    string destiny = ConfigurationManager.AppSettings["Destiny"].ToString();
                    string destinationFile = @destiny;

                    //Variavel que auxiklia pegando o nome dos arquivos
                    string fileName = string.Empty;
                    string destFile = string.Empty;

                    if (System.IO.Directory.Exists(sourceFile))
                    {
                        
                        string[] files = System.IO.Directory.GetFiles(sourceFile);
                        if (files.Count() > 0) ///Só vai excutar cópia caso haja algum arquivo na origem
                        {
                            Log.Debug("Tem um arquivos para copiar!" + DateTime.Now);
                            if (System.IO.Directory.Exists(destinationFile))
                            {
                                Log.Debug("Diretório de destino Existe!" + DateTime.Now);
                                string[] filesDestination = System.IO.Directory.GetFiles(destinationFile);                                
                                try
                                {
                                    Log.Debug("Iniciando exclusão de todos os arquivos do diretorio de destino!" + DateTime.Now);
                                    foreach (string s in filesDestination)
                                    {
                                        System.IO.File.Delete(s);
                                    }
                                    Log.Debug("Diretório de destino sem nenhum arquivo!" + DateTime.Now);
                                }
                                catch (System.IO.IOException e)
                                {
                                    Log.Error("Arquivo no diretório de destino não pôde ser excluído!" + DateTime.Now);
                                    Log.Error("Erro " + e.ToString());
                                    return;
                                }

                            }
                            else
                            {
                                Log.Debug("Diretório de destino inexistente....... criando diretório!" + DateTime.Now);
                                System.IO.Directory.CreateDirectory(destinationFile);
                            }
                            try
                            {
                                Log.Debug("Copiando arquivos para diretório de destino" + DateTime.Now);
                                foreach (string s in files)
                                {
                                    
                                    fileName = System.IO.Path.GetFileName(s);
                                    destFile = System.IO.Path.Combine(destinationFile, fileName);                                    
                                    System.IO.File.Copy(s, destFile, true);
                                }
                            }
                            catch (System.IO.IOException e)
                            {
                                Log.Error("Arquivo no diretório de destino não pôde ser copiado!" + DateTime.Now);
                                Log.Error("Erro " + e.ToString());
                                return;
                            }
                            Log.Debug("Preparando para iniciar a exclusão de arquivos do diretório de origem...." + DateTime.Now);
                            try
                            {
                                Log.Debug("Excluindo arquivos para diretório de destino" + DateTime.Now);
                                foreach (string s in files)
                                {                                    
                                    System.IO.File.Delete(s);
                                }
                            }
                            catch (System.IO.IOException e)
                            {
                                Log.Error("Arquivo no diretório de origem não pôde ser excluído!" + DateTime.Now);
                                Log.Error("Erro " + e.ToString());
                                return;
                            }
                        }
                        else
                        {
                            Log.Debug("Não tem arquivos para copiar!" + DateTime.Now);
                        }
                    }

                    System.Threading.Thread.Sleep(timer);
                }
                
            }
            catch (Exception ex)
            {
                Log.Error("Erro " + ex.ToString());
                Process();
            }
            finally
            {
                Log.Debug("Finalizou Thread");
            }
        }

    }
}
