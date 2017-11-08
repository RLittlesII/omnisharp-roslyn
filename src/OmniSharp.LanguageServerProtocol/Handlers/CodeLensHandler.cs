using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Mef;
using OmniSharp.Models.CodeAction;

namespace OmniSharp.LanguageServerProtocol.Handlers
{
    public class CodeLensHandler : ICodeLensHandler
    {
        private readonly IRequestHandler<RunCodeActionRequest, RunCodeActionResponse> _runCodeActionHandler;
        private readonly IRequestHandler<GetCodeActionRequest, GetCodeActionsResponse> _getCodeActionHandler;
        private readonly OmniSharpWorkspace _workspace;

        public CodeLensHandler(Mef.IRequestHandler<RunCodeActionRequest, RunCodeActionResponse> runCodeActionHandler, IRequestHandler<GetCodeActionRequest, GetCodeActionsResponse> getCodeActionHandler,
            OmniSharpWorkspace workspace)
        {
            _runCodeActionHandler = runCodeActionHandler;
            _getCodeActionHandler = getCodeActionHandler;
            _workspace = workspace;
        }

        public async Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken token)
        {
            var getCodeActionRequest = new GetCodeActionRequest
            {
            };

            var omnisharpResponse = await _getCodeActionHandler.Handle(getCodeActionRequest);
            return new CodeLensContainer();
        }

        public CodeLensRegistrationOptions GetRegistrationOptions()
        {
            throw new NotImplementedException();
        }

        public void SetCapability(CodeLensCapability capability)
        {
            throw new NotImplementedException();
        }
    }
}
