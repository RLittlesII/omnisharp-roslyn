using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.LanguageServerProtocol.Handlers
{
    /// <summary>
    /// This needs to apply work space edits in order to update the current working document.
    /// </summary>
    /// <seealso cref="OmniSharp.Extensions.LanguageServer.Protocol.IExecuteCommandHandler" />
    internal class RunCodeActionHandler : IExecuteCommandHandler
    {
        public async Task Handle(ExecuteCommandParams request, CancellationToken token)
        {
            //TODO: Call the Apply Workspace Edit function before returning.
            await Task.CompletedTask;
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions()
        {
            return new ExecuteCommandRegistrationOptions
            {
            };
        }

        public void SetCapability(ExecuteCommandCapability capability) { throw new NotImplementedException(); }
    }
}
