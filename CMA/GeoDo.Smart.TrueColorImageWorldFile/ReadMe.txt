对风云三号真彩色图片配准：即根据文件名，生成地理坐标信息

                new Regex(@"^FY3[A|B]_MERSI_(?<Code>T\d{3})_L2_PAD_MLT_(?<Projection>GLL)_\d{8}_\d{4}_(?<Resolution>[0250])M_MS_03_02_01.JPG$", RegexOptions.Compiled),
                new Regex(@"^FY3[A|B]_MERSI_(?<Code>T\d{3})_L2_PAD_MLT_(?<Projection>GLL)_\d{8}_POAD_(?<Resolution>[0250])M_MS_03_02_01.JPG$", RegexOptions.Compiled)