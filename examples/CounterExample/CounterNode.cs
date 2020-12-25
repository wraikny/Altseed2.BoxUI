using System;
using System.Collections.Generic;
using Altseed2;
using Altseed2.BoxUI;

namespace CounterExample
{
    sealed class CounterNode : Node
    {
        readonly State _state;
        readonly Queue<IMsg> _msgQueue;
        readonly View _view;

        readonly BoxUIRootNode _uiRootNode;
        
        public CounterNode()
        {
            _state = new State();
            _view = new View();
            _msgQueue = new Queue<IMsg>();
            _uiRootNode = new BoxUIRootNode();

            var cursor = new BoxUIMouseCursor();

            _uiRootNode.Cursors.Add(cursor);
            _uiRootNode.SetElement(_view.MakeView(_state, _msgQueue.Enqueue));

            AddChildNode(_uiRootNode);
            AddChildNode(cursor);
        }

        protected override void OnUpdate()
        {
            if (_msgQueue.Count > 0)
            {
                while (_msgQueue.TryDequeue(out var msg))
                {
                    msg.Update(_state);
                }

                // 新しいElementを作成する前にClearElementを呼び出す。
                _uiRootNode.ClearElement();
                var element = _view.MakeView(_state, _msgQueue.Enqueue);
                _uiRootNode.SetElement(element);
            }
        }
    }
}