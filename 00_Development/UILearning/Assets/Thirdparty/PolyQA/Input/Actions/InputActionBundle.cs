using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PolyQA.Input.Actions
{
    public class InputActionBundle : IInputAction
    {
        private readonly RuntimeContext _context;
        private readonly InputActionRequest _request;
        private List<IInputAction> _actions;

        public InputActionBundle(RuntimeContext context, InputActionRequest request)
        {
            _context = context;
            _request = request;
        }

        public IEnumerator Process(IInputActionContext context)
        {
            var subActions = _request.Sequences
                .Select<IInputActionSequence, IInputAction>(x => x switch
                {
                    InputTouchActionSequence y => new UnityInputTouchAction(_context, y),
                    InputTouchRecordSequence y => new UnityInputTouchRecordAction(_context, y),
                    InputTextSequence y => new UnityInputTextAction(_context, y),
                    _ => null
                })
                .Where(x => x != null)
                .Select(context.StartSubAction);

            foreach (var action in subActions)
            {
                yield return action;
            }
        }
    }
}