using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Activation.Blocks;
using XSockets.Core.Common.Socket;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;
using XSockets.Plugin.Framework;
using NgDataSync.Core.Common.Paging;
using NgDataSync.Core.Common.Validation;
using NgDataSync.Core.Interfaces.Service;
using NgDataSync.Core.Model;
using NgDataSync.Core.ViewModel;
using NgDataSync.NinjectModules;
using System.Threading.Tasks;

namespace NgDataSync.XSocketsModules
{
        /// <summary>
    /// XSockets sample controller for basic DataSync operations.    
    /// </summary>
    /// <typeparam name="T">XSockets controller</typeparam>
    /// <typeparam name="TM">Entity Model</typeparam>
    /// <typeparam name="TVM">Entity View Model</typeparam>
    /// <typeparam name="TS">Service</typeparam>
    public abstract class XSocketsDataSyncController<T, TM, TVM, TS> : XSocketController
        where T : class, IXSocketController
        where TM : PersistentEntity
        where TVM : BaseViewModel
        where TS : IService<TM, TVM>
    {
        private T _controller;
        protected readonly string Entity = typeof(TM).Name;
        protected static readonly IKernel kernel;

        /// <summary>
        /// Sync commands... Just to avoid string in code
        /// </summary>
        public static class DataSyncCommand
        {
            public const string Add = "add";
            public const string Update = "update";
            public const string Delete = "delete";
            public const string Init = "init";
            public const string Error = "Error";
            public const string Page = "page";
        }

        static XSocketsDataSyncController()
        {
            kernel = new StandardKernel(new ServiceModule());
        }

        /// <summary>
        /// To get the correct controller type, cant use abstract class when sending data
        /// </summary>
        protected T Controller
        {
            get { return _controller ?? (_controller = (T)Composable.GetExport<IXSocketController>(typeof(T))); }
        }

        /// <summary>
        /// Will fetch and send all data in the repository for each topic sent in with the connection.
        /// </summary>
        public override async Task OnOpened()
        {
            using (var block = new ActivationBlock(kernel))
            {
                var s = block.Get<TS>();
                //Send back the data in the repo for the type TV
                await this.Invoke(s.GetAll(), string.Format("{0}:{1}", DataSyncCommand.Init, Entity));
            }
        }

        /// <summary>
        /// Search the repository, not exposed to clients since accessor is protected
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TM> Find(Func<TM, bool> expression)
        {
            using (var block = new ActivationBlock(kernel))
            {
                var s = block.Get<TS>();
                //Send back the data in the repo for the type TV
                return s.Find(expression);
            }

        }

        /// <summary>
        /// Adds/Updates the data from in reposiotry and tells all (subscribing) clients about it.
        /// 
        /// Override to implement custom logic
        /// </summary>
        /// <param name="model"></param>
        public virtual async Task Update(TM model)
        {
            var command = DataSyncCommand.Update;
            if (model.Id == 0)
            {
                command = DataSyncCommand.Add;
            }

            using (var block = new ActivationBlock(kernel))
            {
                var s = block.Get<TS>();
                //Send back the data in the repo for the type TV
                var result = s.SaveOrUpdate(model);

                if (result.IsValid)
                {
                    //Ok, tell all clients about the update
                    result.SetViewModel();
                    await Sync(command, result.EntityViewModel);
                }
                else
                {
                    //Could not save, send information back to caller
                    await this.Invoke(result, DataSyncCommand.Error);
                }
            }
        }

        /// <summary>
        /// Deletes the data from the reposiotry and tells all (subscribing) clients about it
        /// 
        /// Override to implement custom logic
        /// </summary>
        /// <param name="model"></param>
        public virtual async Task Delete(TVM model)
        {
            using (var block = new ActivationBlock(kernel))
            {
                var s = block.Get<TS>();             
                s.Delete(model.Id);
                await Sync(DataSyncCommand.Delete, model);
            }
        }

        public async Task Page(int page, int pageSize)
        {
            using (var block = new ActivationBlock(kernel))
            {
                var s = block.Get<TS>();
                var entities = new List<TVM>();
                var p = s.Page(page, pageSize);
                foreach (var entity in p.Entities)
                {
                    var vc = entity.GetValidationContainer(default(TVM));
                    vc.SetViewModel();
                    entities.Add(vc.EntityViewModel);
                }
                await this.Invoke(new Page<TVM>(entities, p.Count, p.PageSize, p.CurrentPage),
                    string.Format("{0}:{1}", DataSyncCommand.Page, Entity));
            }
        }

        /// <summary>
        /// Will do a PUBLISH of changes by default, override to implement specific logic and/or RPC
        /// </summary>
        /// <param name="command"></param>
        /// <param name="model"></param>
        protected virtual async Task Sync(string command, TVM model)
        {
            await Controller.PublishToAll(model, string.Format("{0}:{1}", command, Entity));
        }
    }
}