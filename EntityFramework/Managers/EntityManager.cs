using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Linq;

namespace EntityFramework.Managers
{
    public class EntityManager
    {
        // Dictionary all the things!    
        private TagManager<uint> tagManager;
        private GroupManager<uint> groupManager;
        private Dictionary<uint, Entity> entities;
        private Dictionary<string, Entity> entityNames;
        private Dictionary<uint, BitVector> componentBitVectors;
        private Dictionary<uint, BitVector> systemBitVectors;
        private Dictionary<uint, Dictionary<int, Component>> entityComponents;
        private TypeMapper<Component> componentTypeMapper;
        internal World world;


        public TagManager<uint> TagManager
        {
            get
            {
                return tagManager;
            }
        }

        public GroupManager<uint> GroupManager
        {
            get
            {
                return groupManager;
            }
        }

        internal EntityManager(World world)
        {
            tagManager = new TagManager<uint>();
            groupManager = new GroupManager<uint>();
            entities = new Dictionary<uint, Entity>();
            entityNames = new Dictionary<string, Entity>();
            entityComponents = new Dictionary<uint, Dictionary<int, Component>>();
            componentTypeMapper = Mappers.ComponentTypeMapper;
            componentBitVectors = new Dictionary<uint, BitVector>();
            systemBitVectors = new Dictionary<uint, BitVector>();
            this.world = world;
        }

        /// <summary>
        /// Creates a new Entity but does NOT add it to the world until the entitie's Refresh() method is called
        /// </summary>
        /// <returns>New Entity</returns>
        public Entity CreateEntity()
        {
            Entity e = new Entity(this);
            entities.Add(e.Id, e);
            return e;
        }

        public void Refresh(uint id)
        {
            world.Refresh(entities[id]);
        }
        public void SetEntityName(string name, uint id)
        {
            if (name != null && entities.ContainsKey(id))
            {
                if (entities[id].Name != null)
                    if (entityNames.ContainsKey(entities[id].Name))
                        entityNames.Remove(entities[id].Name);
                entityNames.Add(name, entities[id]);
                entities[id].setName(name);
            }
        }

        public Entity GetEntityByName(string name)
        {
            if (entityNames.ContainsKey(name))
                return entityNames[name];
            return null;
        }

        public void SerializeEntities(String filePath)
        {
            if (Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                XmlWriterSettings sett = new XmlWriterSettings();
                sett.Indent = true;
                sett.IndentChars = "\t";

                XmlWriter writer = XmlWriter.Create(filePath, sett);
                writer.WriteStartElement("root");

                XmlSerializer xs = new XmlSerializer(typeof(Component), Component.ComponentTypes.ToArray());
                int numFailed = 0;
                int num = 0;
                foreach (uint key in entities.Keys)
                {
                    Entity e = entities[key];

                    writer.WriteStartElement("Entity");
                    writer.WriteAttributeString("guid", e.GUID.ToString());
                    if (e.Name != null)
                        writer.WriteAttributeString("name", e.Name);
                    if(e.DataEntity)
                        writer.WriteAttributeString("data", "true");

                    Dictionary<int, Component> d = entityComponents[key];
                    foreach (Component c in d.Values)
                    {
                        num++;
                        try
                        {
                            xs.Serialize(writer, c);
                        }
                        catch (InvalidOperationException)
                        {
                            numFailed++;
                        }
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }

        }

        public void DeserializeEntities(String filePath)
        {
            if (File.Exists(filePath))
            {
                XmlReader read = XmlReader.Create(filePath);

                read.ReadStartElement();

                XmlSerializer xs = new XmlSerializer(typeof(Component), Component.ComponentTypes.ToArray());
                int numFailed = 0;
                int num = 0;

                while (read.IsStartElement())
                {
                    //read attributes first
                    string gid = read.GetAttribute("guid");
                    Guid guid = gid == null ? Guid.NewGuid() : Guid.Parse(gid);
                    String name = read.GetAttribute("name");
                    String data = read.GetAttribute("data");
                    Entity e = new Entity(this, guid);
                    entities.Add(e.Id, e);
                    if (name != null)
                        e.Name = name;
                    if (data == "true")
                        e.DataEntity = true;

                    read.ReadStartElement();//<Entity>


                    while (read.IsStartElement())//<Component>
                    {
                        Component c = (Component)xs.Deserialize(read);
                        c.DidDeserialize();
                        e.AddComponent(c);
                    }

                    e.Refresh();

                    read.ReadEndElement();//</Entity>
                }
            }
        }

        public void DestroyEntity(uint id)
        {
            if (!entities.ContainsKey(id))
            {
                return;
            }
            if (entities[id].Name != null)
                if (entityNames.ContainsKey(entities[id].Name))
                    entityNames.Remove(entities[id].Name);
            entities.Remove(id);
            groupManager.RemoveFromAllGroups(id);
            tagManager.RemoveTag(id);
            componentBitVectors.Remove(id);
            systemBitVectors.Remove(id);
            entityComponents.Remove(id);
        }

        public Entity GetEntity(uint id)
        {
            if (!entities.ContainsKey(id))
            {
                return null;
            }
            return entities[id];
        }

        public void AddComponent(uint id, Component c)
        {
            if (!entityComponents.ContainsKey(id))
            {
                entityComponents.Add(id, new Dictionary<int, Component>());
            }
            else if (HasComponent(id, c.Name))
            {
                RemoveComponent(id, c.Name);
            }
            entityComponents[id].Add(c.Id, c);
            GetComponentBitVector(id).AddBit(c.Bit);
        }
        /// <summary>
        /// Returns true if Entity contains a component of specified Type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasComponent(uint id, Type type)
        {
            if (entityComponents.ContainsKey(id))
            {
                foreach (Component c in entityComponents[id].Values)
                {
                    if (c.GetType() == type)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns true if specified entity containts the named component
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool HasComponent(uint id, string Name)
        {
            if (entityComponents.ContainsKey(id))
            {
                if (entityComponents[id].ContainsKey(Name.GetHashCode()))
                {
                    return true;
                }
            }
            return false;
        }
        public Component GetComponent(uint id, string name)
        {
            if (entityComponents.ContainsKey(id))
            {
                if (entityComponents[id].ContainsKey(name.GetHashCode()))
                    return entityComponents[id][name.GetHashCode()];

            }
            return null;
        }
        /// <summary>
        /// Returns the first available component of specified type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Component GetComponent(uint id, Type type)
        {
            if (entityComponents.ContainsKey(id))
            {
                foreach (Component c in entityComponents[id].Values)
                {
                    if (c.GetType() == type)
                        return c;
                }
            }
            return null;
            /*
            if (entityComponents[id].ContainsKey(componentTypeMapper.GetValue(type)))
            {
                return entityComponents[id][componentTypeMapper.GetValue(type)];
            }
            else if (entityComponents[id].ContainsKey(componentTypeMapper.GetValue(type.BaseType)))
            {
                return entityComponents[id][componentTypeMapper.GetValue(type.BaseType)];
            }
            else
            {
                return null;
            }*/

        }

        public IEnumerable<Component> GetComponents(uint id)
        {
            if (entityComponents.ContainsKey(id))
            {
                return entityComponents[id].Values;
            }
            return new List<Component>();
        }

        public void RemoveComponent(uint id, string name)
        {
            /*if (!HasComponent(id, name))
            {
                return;
            }*/
            Component c = GetComponent(id, name);
            entityComponents[id].Remove(name.GetHashCode());
            if (c != null)
                if (!HasComponent(id, c.GetType()))
                    GetComponentBitVector(id).RemoveBitByOffset((int)componentTypeMapper.GetValue(c.GetType()));
        }
        public void RemoveComponent(uint id, Component c)
        {
            entityComponents[id].Remove(c.Name.GetHashCode());
            if (!HasComponent(id, c.GetType()))
                GetComponentBitVector(id).RemoveBitByOffset((int)componentTypeMapper.GetValue(c.GetType()));
        }

        public BitVector GetComponentBitVector(uint id)
        {
            if (!componentBitVectors.ContainsKey(id))
            {
                componentBitVectors[id] = new BitVector();
            }
            return componentBitVectors[id];
        }

        public BitVector GetSystemBitVector(uint id)
        {
            if (!systemBitVectors.ContainsKey(id))
            {
                systemBitVectors[id] = new BitVector();
            }
            return systemBitVectors[id];
        }

        public void RemoveAllEntities()
        {
            while (entities.Count > 0)
                world.DestroyEntity(entities.Values.ElementAt(0));
        }
    }
}
