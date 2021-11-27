using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Content.Pipeline.Audio;

namespace Conarium.Services
{
    /// <summary>
    /// A method to generate / return effect per mesh part.
    /// </summary>
    /// <param name="modelPath">Model path this part belongs to.</param>
    /// <param name="modelContent">Loaded model raw content.</param>
    /// <param name="part">Part instance we want to create effect for.</param>
    /// <returns>Effect instance or null to use default.</returns>
    public delegate Effect EffectsGenerator(string modelPath, ModelContent modelContent, ModelMeshPart part);


    public struct AssetRequest
    {
        public string Path;
        public Type ObjectType;
    
        public AssetRequest(string path)
        {
            Path = path;
            ObjectType = null;
        }

        public static AssetRequest Create<T>(string path)
        {
            return new AssetRequest(path)
            {
                ObjectType = typeof(T) 
            };
        }
    }

    public class AssetService : Service
    {

        public static AssetService Get() => Instance;
		public static AssetService Instance {get; private set;}

		#region Content Management Instances


        // graphics device
        GraphicsDevice _graphics;

        // context objects
        PipelineImporterContext _importContext;
        PipelineProcessorContext _processContext;

        // importers
        OpenAssetImporter _openImporter;
        EffectImporter _effectImporter;
        FontDescriptionImporter _fontImporter;
        Dictionary<string, ContentImporter<AudioContent>> _soundImporters = new Dictionary<string, ContentImporter<AudioContent>>();

        // processors
        ModelProcessor _modelProcessor;
        EffectProcessor _effectProcessor;
        FontDescriptionProcessor _fontProcessor;

		#endregion

		// default effect to assign to all meshes 
        public BasicEffect DefaultEffect;
        public EffectsGenerator EffectsGenerator;

        // loaded assets caches
        Dictionary<string, Object> _loadedAssets = new Dictionary<string, Object>();

        Queue<AssetRequest> _assetQueue = new Queue<AssetRequest>();
        

        public AssetService(Game game) : base(game)
        {
			Instance = this; // set service instance;


            _graphics = game.GraphicsDevice;

            _openImporter = new OpenAssetImporter();
            _effectImporter = new EffectImporter();
            _soundImporters[".wav"] = new WavImporter();
            _soundImporters[".ogg"] = new OggImporter();
            _soundImporters[".wma"] = new WmaImporter();
            _soundImporters[".mp3"] = new Mp3Importer();
            _fontImporter = new FontDescriptionImporter();

            string projectDir = "_proj";
            string outputDir = "_output";
            string intermediateDir = "_inter";
            var pipelineManager = new PipelineManager(projectDir, outputDir, intermediateDir);

            _importContext = new PipelineImporterContext(pipelineManager);
            _processContext = new PipelineProcessorContext(pipelineManager, new PipelineBuildEvent());

            _modelProcessor = new ModelProcessor();
            _effectProcessor = new EffectProcessor();
            _fontProcessor = new FontDescriptionProcessor();

            DefaultEffect = new BasicEffect(_graphics);
        }


		private string GetFileNameFromPath(string path) => Path.GetFileName(path);
		

        /// <summary>
        /// Validate given path, and attempt to return from cache.
        /// Will return true if found in cache, false otherwise (fromCache will return result).
        /// Will throw exceptions if path not found, or cache contains wrong type.
        /// </summary>
        private bool ValidatePathAndGetCached<T>(string assetPath, out T fromCache) where T : class
        {
            // get from cache
            if (_loadedAssets.TryGetValue(assetPath, out object cached))
            {
                fromCache = cached as T;
                var expected = (typeof(T).Name);
                var foundType = cached.GetType().Name;
                if (fromCache == null)
                    throw new InvalidOperationException($"Asset found in cache, but gives wrong type! Expected Type: '{expected}', found: '{foundType}'");

                return true;
            }

            // make sure file exists
            if (!File.Exists(assetPath))
                throw new FileNotFoundException($"{(typeof(T).Name)} asset file '{assetPath}' not found! Fullpath: {Path.GetFullPath(assetPath)}", assetPath);

            // not found in cache
            fromCache = null;
            return false;
        }


		private void CacheAsset<T>(string path, T Asset) 
		{			
			_loadedAssets[path] = Asset;
		}


		// allow objects to be garbage collected
        public void ClearCache()
        {
			_loadedAssets.Clear();
        }

        public void QueueAsset<T>(string path)
        {
            _assetQueue.Enqueue(AssetRequest.Create<T>(path));
        }

        private void CorrectAlphaPremultiply(ref Texture2D Asset)
        {
            Color[] data = new Color[Asset.Width * Asset.Height];
            Asset.GetData(data);
            for (int i = 0; i != data.Length; i++)
                data[i] = Color.FromNonPremultiplied(data[i].ToVector4());
            Asset.SetData(data);
        }
        private Texture2D FromStream(GraphicsDevice GFX, Stream DataStream)
        {
            Texture2D Content = Texture2D.FromStream(GFX, DataStream);
            CorrectAlphaPremultiply(ref Content);
            return Content;
        }

        

        public Texture2D LoadTexture(string filepath)
        {
            // pull from cache if avaliable
            if (ValidatePathAndGetCached<Texture2D>(filepath, out Texture2D cached))
                return cached;

            
            Texture2D tex = FromStream(_graphics, TitleContainer.OpenStream(filepath));
            CacheAsset<Texture2D>(filepath, tex);
            return tex;
        }

        // FIXME: Currently not working, missing libfreetype6 or some shit...
        public SpriteFont LoadSpriteFont(string fontPath) {
            if (ValidatePathAndGetCached<SpriteFont>(fontPath, out SpriteFont cached))
                return cached;

            // import spritefont.xml file
            var fontDescriptor = _fontImporter.Import(fontPath, _importContext);
            var spriteFontContent = _fontProcessor.Process(fontDescriptor, _processContext);

            // create spritefont
            var textureContent = spriteFontContent.Texture.Mipmaps[0];
            textureContent.TryGetFormat(out SurfaceFormat format);
            Texture2D texture = new Texture2D(_graphics, textureContent.Width, textureContent.Height, false, format);
            texture.SetData(textureContent.GetPixelData());
            List<Rectangle> glyphBounds = spriteFontContent.Glyphs;
            List<Rectangle> cropping = spriteFontContent.Cropping;
            List<char> characters = spriteFontContent.CharacterMap;
            int lineSpacing = spriteFontContent.VerticalLineSpacing;
            float spacing = spriteFontContent.HorizontalSpacing;
            List<Vector3> kerning = spriteFontContent.Kerning;
            char? defaultCharachter = spriteFontContent.DefaultCharacter;
            var sf = new SpriteFont(texture, glyphBounds, cropping, characters, lineSpacing, spacing, kerning, defaultCharachter);
            CacheAsset<SpriteFont>(fontPath, sf);
            return sf;
        }


        public SoundEffect LoadSound(string path)
        {

			// pull from cache if avaliable
			if (ValidatePathAndGetCached<SoundEffect>(path, out SoundEffect cached))
				return cached;

            var extension = Path.GetExtension(path).ToLower();
            if (!_soundImporters.ContainsKey(extension))
            {
                throw new InvalidContentException($"Invalid sound file type '{extension}'. can only load sound files of types: '{string.Join(',', _soundImporters.Keys)}'");
            }

            // import audio
            var audioContent = _soundImporters[extension].Import(path, _importContext);

            // create sound & return
            byte[] data = new byte[audioContent.Data.Count];
            audioContent.Data.CopyTo(data, 0);
            AudioChannels pickMonoOrStereo = audioContent.Format.ChannelCount == 1 ? AudioChannels.Mono : AudioChannels.Stereo; 
            SoundEffect sound = new SoundEffect(data, 0, 
				data.Length, 
				audioContent.Format.SampleRate, 
            	pickMonoOrStereo, 
				audioContent.LoopStart, 
				audioContent.LoopLength);

			// cache and return
			CacheAsset<SoundEffect>(path, sound);
			return sound;
        }

        public Song LoadSong(string path)
        {
			// pull from cache if avaliable
			if (ValidatePathAndGetCached<Song>(path, out Song cached))
				return cached;

			// load song 
			var name = Path.GetFileNameWithoutExtension(path);
			var song = Song.FromUri(name, new Uri(path));


			// add to cache and return
			CacheAsset<Song>(path, song);
			return song;
        }


       	/// <summary>
        /// Load a shader effect from path.
        /// Note: requires the mgfxc dll to work.
        /// </summary>
        /// <param name="effectFile">Effect file path.</param>
        /// <returns>MonoGame Shader Effect.</returns>
        public Effect LoadEffectShader(string shaderPath)
        {
			if (ValidatePathAndGetCached<Effect>(shaderPath, out Effect cached))
				return cached;

			// create shader effect object
			var shaderContent = _effectImporter.Import(shaderPath, _importContext);
			var shaderData = _effectProcessor.Process(shaderContent, _processContext);
			var dataBuffer = shaderData.GetEffectCode();
			var effect = new Effect(_graphics, dataBuffer, 0, dataBuffer.Length);

			CacheAsset<Effect>(shaderPath, effect);

			return effect;
        }


		public Effect LoadCompiledEffectShader(string shaderPath)
		{
			if (ValidatePathAndGetCached<Effect>(shaderPath, out Effect cached))
				return cached;

			// create shader and "compile" lol
			byte[] bytecode = File.ReadAllBytes(shaderPath);
			var effect = new Effect(_graphics, bytecode, 0, bytecode.Length);

			CacheAsset<Effect>(shaderPath, effect);
			return effect;
		}

        public Model LoadModel(string path)
        {
            var node = _openImporter.Import(path, _importContext);
            ModelContent modelContent = _modelProcessor.Process(node, _processContext);

            // sanity
            if (modelContent.Meshes.Count == 0)
                throw new FormatException("Model file contains 0 meshes (Is it corrupted?)");

            // extract bones
            var bones = new List<ModelBone>();
            foreach (var boneContent in modelContent.Bones)
            {
                var bone = new ModelBone
                {
                    Transform = boneContent.Transform,
                    Index = bones.Count,
                    Name = boneContent.Name,
                    ModelTransform = modelContent.Root.Transform,
                };
                bones.Add(bone);
            }

            // resolve bone hierarchy
            for (var index = 0; index < bones.Count; ++index)
            {
                var bone = bones[index];
                var content = modelContent.Bones[index];

                if (content.Parent != null && content.Parent.Index != -1)
                {
                    bone.Parent = bones[content.Parent.Index];
                    bone.Parent.AddChild(bone);
                }
            }

            // extract meshes
            var meshes = new List<ModelMesh>();
            foreach (var meshContent in modelContent.Meshes)
            {
                var name = meshContent.Name;
                var parentBoneIndex = meshContent.ParentBone.Index;
                var boundingSphere = meshContent.BoundingSphere;
                var meshTag = meshContent.Tag;

                // extract parts
                var parts = new List<ModelMeshPart>();
                foreach (var partContent in meshContent.MeshParts)
                {
                    // build index buffer

                    IndexBuffer indexBuffer = new IndexBuffer(_graphics, IndexElementSize.ThirtyTwoBits, partContent.IndexBuffer.Count, BufferUsage.WriteOnly);
                    {
                        Int32[] data = new Int32[partContent.IndexBuffer.Count];
                        partContent.IndexBuffer.CopyTo(data, 0);
                        indexBuffer.SetData(data);

                    }
                    // build vertex buffer
                    var vbDeclareContent = partContent.VertexBuffer.VertexDeclaration;

                    List<VertexElement> elements = new List<VertexElement>();

                    foreach (var declareContentElement in vbDeclareContent.VertexElements)
                    {
                        elements.Add(new VertexElement(
                            declareContentElement.Offset,
                            declareContentElement.VertexElementFormat,
                            declareContentElement.VertexElementUsage,
                            declareContentElement.UsageIndex
                        ));
                    }
                    var vbDeclare = new VertexDeclaration(elements.ToArray());

                    VertexBuffer vertexBuffer = new VertexBuffer(_graphics, vbDeclare, partContent.NumVertices, BufferUsage.WriteOnly);
                    {
                        vertexBuffer.SetData(partContent.VertexBuffer.VertexData);
                    };

                    // create and add part
#pragma warning disable CS0618 // FIXME: type member obsolete?
                    ModelMeshPart part = new ModelMeshPart()
                    {
                        VertexOffset = partContent.VertexOffset,
                        NumVertices = partContent.NumVertices,
                        PrimitiveCount = partContent.PrimitiveCount,
                        StartIndex = partContent.StartIndex,
                        Tag = partContent.Tag,
                        IndexBuffer = indexBuffer,
                        VertexBuffer = vertexBuffer
                    };
#pragma warning restore CS0618
                    parts.Add(part);
                }

                // create and add mesh to meshes list
                var mesh = new ModelMesh(_graphics, parts)
                {
                    Name = name,
                    BoundingSphere = boundingSphere,
                    Tag = meshTag
                };
                meshes.Add(mesh);

                // set parts effect
                // NOTE: this must come after we add parts to the mesh otherwise u get exceptions
                foreach (var part in parts)
                {
                    var effect = EffectsGenerator != null ? EffectsGenerator(path, modelContent, part) ?? DefaultEffect : DefaultEffect;
                    part.Effect = effect;
                }

                // add to parent bone
                if (parentBoneIndex != -1)
                {
                    mesh.ParentBone = bones[parentBoneIndex];
                    mesh.ParentBone.AddMesh(mesh);
                }
            }

            // create model
            var model = new Model(_graphics, bones, meshes);
            model.Root = bones[modelContent.Root.Index];
            model.Tag = modelContent.Tag;

            // we need to call BuildHierarchy() but its internal,
            // so we use reflection to access it ¯\_(ツ)_/¯
            var methods = model.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var BuildHierarchy = methods.Where(x => x.Name == "BuildHierarchy" && x.GetParameters().Length == 0).First();
            BuildHierarchy.Invoke(model, null);

            // add to cache and return
            _loadedAssets[path] = model;
            return model;

        }
    }
}