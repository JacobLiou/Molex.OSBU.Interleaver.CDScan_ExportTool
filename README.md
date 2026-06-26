# CD_Scan 导表工具

Interleaver（交织器）终测 CD（色散）工序的数据采集与导表工具。在自动化测试过程中产生大量光谱扫描数据，本工具负责从 Agilent CD 测试仪采集原始数据，并按产品脚本计算 ITU 信道级指标，输出标准导表 CSV，供后续分析与判定使用。

当前版本：窗体标题 `CD_Scan_1.4.0(20190424)`

## 技术栈

- .NET Framework 4.6.1
- C# / WinForms

## 构建与运行

1. 用 Visual Studio 打开 [`CD_Scan.sln`](CD_Scan.sln)
2. 编译 `CD_Scan` 项目（Debug 或 Release）
3. 输出目录：`CD_Scan/bin/Debug/` 或 `CD_Scan/bin/Release/`

**外部依赖**（需与 `CD_Scan.exe` 同目录，仓库中可能不含）：

- `Ag86038x_Engine.dll`
- `InstrumentObjects.dll`
- `RemoteClient.dll`

## 功能概览

| 按钮 | 功能 |
|------|------|
| 连接设备 | 读取/保存 `CD_IP.txt`，连接 Agilent CD 测试仪 |
| 归零 | 仪器 Normalize |
| 扫描 | 按 ODD/EVEN（及 Mux/Demux）采集并保存原始 CSV |
| 导表 | 按 `Temp_config.txt` 中的路径与脚本生成 `*-导表.csv` |
| 原始数据 / 导表配置文件 | 独立导表时手动选择原始 CSV 与脚本 |
| 独立导表 | 不依赖自动路径的导表 |
| 预匹配 | COER 与 DCM 测试数据叠加匹配（需 `amalgamate.ini`、`parameter.ini`） |

## 数据流程

```
仪器扫描 → 原始 CSV（Freq,GD,IL,Phase,PDL,PMD,Freq,CD）
                ↓
         导表脚本（*_script_*.txt）
                ↓
         导表 CSV（指标名 + 单位 + MAX-Value + 逐信道数据）
```

## 配置文件

### Temp_config.txt

位于 exe 同目录，GBK 编码，共 4 行逗号分隔：

| 行 | 含义 | 示例字段 |
|----|------|----------|
| 第 1 行 | 扫描参数 | `起始波长,终止波长,步进,RF,IF,PDL开关(0/1),通道间隔` |
| 第 2 行 | 原始文件前缀 | 不含 `-ODD.csv` / `-EVEN.csv` 后缀的完整路径前缀 |
| 第 3 行 | 脚本与模板路径 | `脚本/产品前缀路径,模板目录（含 CD_templet.csv）` |
| 第 4 行 | 产品类型 | `>2` 时启用 Mux/Demux 选项 |

开发样例见 [`CD_Scan/Temp_config.txt`](CD_Scan/Temp_config.txt)。

### 运行时文件（exe 同目录）

| 文件 | 说明 |
|------|------|
| `CD_IP.txt` | CD 测试仪 IP 地址 |
| `Temp_testfile.txt` | 当前测试原始 CSV 路径（扫描/导表时自动写入） |
| `parameter.ini` | 预匹配合格阈值，节名 `8048M_1` / `8048M_2` |
| `amalgamate.ini` | COER/DCM 文件路径与数量配置 |
| `NO_pair.ini` | 预匹配失败时记录的 COER 序列号 |
| `{产品号}.ini` | 产品 `CalGhz` 校准频率（节 `[CalGhz]`，键 `Vaue`） |

> 注意：代码中 `m_nCalGhz` 目前在 `outputresult_Click` 里硬编码为 `184000`，会覆盖 ini 读取值。

## 数据文件格式

### 原始扫描 CSV

文件名约定：`{前缀}-ODD.csv`、`{前缀}-EVEN.csv`（四路产品时含 `-Mux-` / `-Demux-`）

```
Freq,GD,IL/Gain,Phase,PDL,PMD,Freq,CD
196199.252917539,9.455...,...
```

仓库根目录 `*=终测CD测试=*-ODD.csv` 为测试样例。

### 导表脚本

文件名约定：`{产品前缀}_script_{ODD|EVEN}_{PDL|NOPDL}.txt`

示例：[`CFOI050100OPL03_script_ODD_NOPDL.txt`](CFOI050100OPL03_script_ODD_NOPDL.txt)

| 行 | 格式 | 说明 |
|----|------|------|
| 第 1 行 | `行数,列数` | 输出表格维度 |
| 第 2 行 | `首信道,末信道,信道间隔,Demux偏移[,Demux首,Demux末]` | ITU 信道范围 |
| 第 3 行起 | 指标定义行 | 见下表 |

支持的指标类型：

`CCF`、`ITU Shift`、`MAX IL`、`MIN IL`、`Ripple`、`PDL`、`Adj Iso`、`Worst CD`、`GD Ripple`、`GD Slope`、`Passband`、`Stopband`、`PMD`

### 导表输出 CSV

文件名约定：`{前缀}-ODD-导表.csv`、`{前缀}-EVEN-导表.csv`

| 行 | 内容 |
|----|------|
| 第 1 行 | 指标名称（逗号分隔） |
| 第 2 行 | 单位（GHz、dB 等） |
| 第 3 行 | `MAX-Value` 汇总行 |
| 第 4 行 | 空行 |
| 第 5 行起 | 各 ITU 信道的计算值 |

## 典型操作流程

### 扫描 + 导表

1. 编辑 `Temp_config.txt`、`CD_IP.txt`
2. 启动程序 → **连接设备** → **归零**
3. 选择 ODD/EVEN（必要时选 Mux/Demux）→ **扫描**
4. 扫描完成后点击 **导表**，生成 `*-导表.csv`

### 独立导表（已有原始数据）

1. 点击 **原始数据** 选择 `*.csv`
2. 点击 **导表配置文件** 选择 `*_script_*.txt`
3. 点击 **独立导表**

### 预匹配

1. 在 exe 目录配置 `amalgamate.ini`、`parameter.ini`
2. 选择参数 1 或参数 2（读取 `parameter.ini` 对应节）
3. 点击 **预匹配**，程序按 CD 偏差排序 COER，尝试与 DCM 数据叠加并判定是否合格

## 注意事项

- 配置文件与 CSV 多使用 **GBK / gb2312** 编码
- 导表前须存在与产品对应的 `*_script_*.txt` 脚本
- 若目标 `*-导表.csv` 已存在，程序会自动备份为 `*-导表.csv_N.csv`，不直接覆盖
- 仓库根目录大量 `*.csv`、`*_script_*.txt` 为测试样例，非程序配置模板
- 仪器相关功能需在安装 Agilent SDK 且仪器联网的环境下验证

## 源码结构

| 文件 | 职责 |
|------|------|
| [`CD_Scan/Form1.cs`](CD_Scan/Form1.cs) | UI、仪器连接、扫描、导表、预匹配 |
| [`CD_Scan/Calc.cs`](CD_Scan/Calc.cs) | 数据读取、`ILCalc` 指标计算、INI 读写 |
| [`CD_Scan/LineFit.cs`](CD_Scan/LineFit.cs) | 最小二乘与直线拟合 |
| [`CD_Scan/Program.cs`](CD_Scan/Program.cs) | 程序入口 |
