using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class ElementDataBase
{
    ElementData[] m_AllElementData;

    // -----------------------------------------------------------------
    // 
    // -----------------------------------------------------------------
    public void Init()
    {
        m_AllElementData = new ElementData[(int)ElementType._Count];
        string[] allElementsData = Directory.GetFiles("Resources/Elements", "*", SearchOption.AllDirectories);
        foreach(string ElementDataFile in allElementsData)
        {
            ElementData cardData = ResourceLoader.Load<ElementData>(ElementDataFile);
            Debug.Assert(m_AllElementData[(int)cardData.m_Type] == null, "duplicate elements with type '" + cardData.m_Type + "' detected !");
            m_AllElementData[(int)cardData.m_Type] = cardData;
        }
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public ElementType GetRandomElementData()
    {
        int randomMax = 0;
        foreach (ElementData elData in m_AllElementData)
        {
            randomMax += elData.m_RandomWeight;
        }

        int randomGot = RandomManager.GetIntRange(0, randomMax);

        ElementData selectedData = null;
        foreach (ElementData elData in m_AllElementData)
        {
            selectedData = elData;
            randomGot -=  elData.m_RandomWeight;
            if (randomGot <= 0)
            {
                break;
            }
        }

        return selectedData.m_Type;
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public ElementData GetDataFromType(ElementType type)
    {
        Debug.Assert((int)type < (int)ElementType._Count, "Invalid Element '" + type + "' !");
        Debug.Assert(m_AllElementData[(int)type] != null, "Missing elements with type '" + type + "' !");

        return m_AllElementData[(int)type];
    }

	// -----------------------------------------------------------------
	// 
	// -----------------------------------------------------------------
    public ElementType GetRandomBasicElementType()
    {
        int randomMax = 0;
        for (ElementType el = ElementType.Element1; el <= ElementType.Element4; el++)
        {
            randomMax += m_AllElementData[(int)el].m_RandomWeight;
        }

        int randomGot = RandomManager.GetIntRange(0, randomMax);

        ElementData selectedData = null;
        for (ElementType el = ElementType.Element1; el <= ElementType.Element4; el++)
        {
            selectedData = m_AllElementData[(int)el];
            randomGot -=  m_AllElementData[(int)el].m_RandomWeight;
            if (randomGot <= 0)
            {
                break;
            }
        }

        return selectedData.m_Type;
    }
}
