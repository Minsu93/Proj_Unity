using Spine.Unity;
using Spine.Unity.AttachmentTools;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Ionic.Zip;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerGunAttacher : MonoBehaviour
{
    [SerializeField] protected SpineAtlasAsset atlasAsset;
    [SerializeField] protected bool inheritProperties = true;

    Atlas atlas;
    SkeletonRenderer skeletonRenderer;

    string slotName;
    string regionName;

    void Awake()
    {
        skeletonRenderer = GetComponent<SkeletonRenderer>();
        //skeletonRenderer.OnRebuild += Apply;
        //Apply(skeletonRenderer);
    }

    public void ApplyWeapon(string slotName, string regionName)
    {
        this.slotName = slotName;
        this.regionName = regionName;
        Apply(skeletonRenderer);
    }

    void Apply(SkeletonRenderer skeletonRenderer)
    {
        atlas = atlasAsset.GetAtlas();
        if (atlas == null) return;
        if(slotName == null || regionName == null) return;

        float scale = skeletonRenderer.skeletonDataAsset.scale;

        Slot slot = skeletonRenderer.Skeleton.FindSlot(slotName);
        Attachment originalAttachment = slot.Attachment;
        AtlasRegion region = atlas.FindRegion(regionName);


        if (region == null)
        {
            slot.Attachment = null;
        }
        else if (inheritProperties && originalAttachment != null)
        {
            slot.Attachment = originalAttachment.GetRemappedClone(region, true, true, scale);
        }
        else
        {
            RegionAttachment newRegionAttachment = region.ToRegionAttachment(region.name, scale);
            slot.Attachment = newRegionAttachment;
        }
    }

    //void Apply(SkeletonRenderer skeletonRenderer)
    //{
    //    if (!this.enabled) return;

    //    atlas = atlasAsset.GetAtlas();
    //    if (atlas == null) return;
    //    float scale = skeletonRenderer.skeletonDataAsset.scale;

    //    foreach (SlotRegionPair entry in attachments)
    //    {
    //        Slot slot = skeletonRenderer.Skeleton.FindSlot(entry.slot);
    //        Attachment originalAttachment = slot.Attachment;
    //        AtlasRegion region = atlas.FindRegion(entry.region);

    //        if (region == null)
    //        {
    //            slot.Attachment = null;
    //        }
    //        else if (inheritProperties && originalAttachment != null)
    //        {
    //            slot.Attachment = originalAttachment.GetRemappedClone(region, true, true, scale);
    //        }
    //        else
    //        {
    //            RegionAttachment newRegionAttachment = region.ToRegionAttachment(region.name, scale);
    //            slot.Attachment = newRegionAttachment;
    //        }
    //    }
    //}



}
