using System;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace newdemoall
{
    [Serializable]
    [Table(name: "results")]
    public class Result
    {
        [Description("主键")]
        [Key]
        [StringLength(50)]
        [Column(name: "id", TypeName = "varchar")]
        public string Id { get; set; }

        [DefaultValue(0)]
        [Column(name: "is_back")]
        [Description("是否反面,0不是，1是")]
        public int IsBack { get; set; }

        [DefaultValue(0)]
        [Column(name: "score")]
        [Description("缺陷置信度")]
        public float score { get; set; }

        [Column(name: "pcb_id", TypeName = "varchar")]
        [StringLength(50)]
        [Description("对应的pcb板id")]
        public string PcbId { get; set; }

        [Column(name: "area", TypeName = "varchar")]
        [StringLength(250)]
        [Description("区域")]
        public string Area { get; set; }

        [Column(name: "region", TypeName = "varchar")]
        [StringLength(50)]
        [Description("范围 坐标")]
        public string Region { get; set; }

        [Column(name: "ng_type", TypeName = "varchar")]
        [StringLength(100)]
        [Description("Ng种类")]
        public string NgType { get; set; }

        [DefaultValue(0)]
        [Column(name: "is_misjudge")]
        [Description("是否误判, 0否 1是")]
        public int IsMisjudge { get; set; }

        [StringLength(5)]
        [DefaultValue("NG")]
        [Column(name: "result_string", TypeName = "varchar")]
        [Description("人工复验结果")]
        public string ResultString { get; set; }

        [Column(name: "part_image_path", TypeName = "varchar")]
        [StringLength(250)]
        [Description("缺陷的局部图相对于FTP路径的地址")]
        public string PartImagePath { get; set; }

        [Column(name: "create_time")]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

    }
}

