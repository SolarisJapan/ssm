using Godot;
using Game.Core;
using Game.Enums;
using Game.Components;
using Game.Scene;
using System.Collections.Generic;


namespace Game.Entities
{
    public partial class EntityBase : CharacterBody2D
    {
        public ulong EntityId => _entityId;
        public bool IsReady => _ready;

        private ulong _entityId;
        private bool _ready = false;

        // Dont reorder components, as their indexes are cached for fast lookup
        protected List<ComponentBase> _components = [];
        protected Dictionary<System.Type, int> _componentTypeIndexMap = new();

        // GODOT METHODS ----- begin ----
        public override void _EnterTree()
        {

        }

        public override void _Ready()
        {
            _ready = true;
            Init();
            InitializeComponents();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public override void _PhysicsProcess(double delta)
        {
            Update(delta);
        }
        // GODOT METHODS ----- end ----

        public EntityBase()
        {
            _entityId = GameObjID.GetNextID();
        }

        virtual public void OnUpdate(double delta)
        {
        }

        public void EnterScene(SceneBase scene)
        {
            OnEnterScene(scene);
        }

        virtual public void OnEnterScene(SceneBase scene)
        {

        }

        public void ExitScene(SceneBase scene)
        {
            OnExitScene(scene);
        }

        virtual public void OnExitScene(SceneBase scene)
        {
        }

        public void SetDirection(Direction direction)
        {
            GetNode<Sprite2D>("Sprite2D").FlipH = direction == Direction.Backward;
        }

        public T GetComponent<T>() where T : ComponentBase
        {
            if (_componentTypeIndexMap.TryGetValue(typeof(T), out int index))
            {
                return (T)_components[index];
            }

            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T t)
                {
                    _componentTypeIndexMap[typeof(T)] = i;
                    return t;
                }
            }

            return null;
        }

        virtual public void Init()
        {
        }

        protected void Update(double delta)
        {
            UpdateComponents(delta);
            OnUpdate(delta);
        }

        virtual protected void InitializeComponents()
        {
            _components.Add(new MoveComponent(this));
            _components.Add(new StateComponent(this));
            _components.Add(new AnimationComponent(this));

            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Initialize();
            }
        }

        private void UpdateComponents(double delta)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Update(delta);
            }
        }

    }
}
