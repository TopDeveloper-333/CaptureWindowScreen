using System.Linq;
using SharpDX.DXGI;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeskDuplSourceProvider : NotifyPropertyChanged, IVideoSourceProvider
    {
        readonly IVideoSourcePicker _videoSourcePicker;

        public DeskDuplSourceProvider(LanguageManager Loc,
            IVideoSourcePicker VideoSourcePicker,
            IIconSet Icons)
        {
            _videoSourcePicker = VideoSourcePicker;
            Icon = Icons.Game;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public bool PickScreen()
        {
            var screen = _videoSourcePicker.PickScreen();

            return screen != null && Set(screen);
        }

        public bool SelectFirst()
        {
            var output = new Factory1()
                .Adapters1
                .SelectMany(M => M.Outputs
                    .Select(N => new
                    {
                        Adapter = M,
                        Output = N.QueryInterface<Output1>()
                    })).FirstOrDefault();

            if (output == null)
                return false;

            Source = new DeskDuplItem(output.Adapter, output.Output);

            return true;
        }

        public bool Set(IScreen Screen)
        {
            var outputs = new Factory1()
                            .Adapters1
                            .SelectMany(M => M.Outputs
                                .Select(N => new
                                {
                                    Adapter = M,
                                    Output = N.QueryInterface<Output1>()
                                }));

            var match = outputs.FirstOrDefault(M =>
            {
                var r1 = M.Output.Description.DesktopBounds;
                var r2 = Screen.Rectangle;

                return r1.Left == r2.Left
                       && r1.Right == r2.Right
                       && r1.Top == r2.Top
                       && r1.Bottom == r2.Bottom;
            });

            if (match == null)
                return false;

            Source = new DeskDuplItem(match.Adapter, match.Output);

            return true;
        }

        IVideoItem _source;

        public IVideoItem Source
        {
            get => _source;
            private set
            {
                _source = value;
                
                OnPropertyChanged();
            }
        }

        public string Name => "Desktop Duplication";

        public string Description { get; } = @"Faster API for recording screen as well as fullscreen DirectX games.
Not all games are recordable.
Requires Windows 8 or above.
If it does not work, try running Captura on the Integrated Graphics card.";

        public string Icon { get; }

        public override string ToString() => Name;
    }
}