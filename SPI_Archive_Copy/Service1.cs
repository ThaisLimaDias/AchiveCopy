using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace SPI_Archive_Copy
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        Logic logic;
        Thread threadSecundaria;

        public Service1()
        {
            try
            {
                InitializeComponent();
                logic = new Logic();

            }
            catch (Exception ex)
            {
                Log.Error("Erro " + ex.ToString());
            }
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Iniciando Serviço V1.0");
            threadSecundaria = new Thread(x => logic.Process());
            threadSecundaria.Start();


        }
        /// <summary>
        /// Para permitir que o serviço seja iniciado em modo DEBUG
        /// </summary>
        /// <param name="args"></param>
        public void OnStartInDebug(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStop()
        {
            Log.Info("Parando Serviço");
            Log.Info("Aguarde ....");
            logic.Ativo = false;

            while (threadSecundaria.IsAlive) ;
            Log.Info("Serviço Encerrado ....");
        }
    }
}
