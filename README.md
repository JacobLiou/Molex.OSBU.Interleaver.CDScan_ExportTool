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

位于 exe 同目录，GBK 编码，共 4～5 行逗号分隔：

| 行 | 含义 | 示例字段 |
|----|------|----------|
| 第 1 行 | 扫描参数 | `起始波长,终止波长,步进,RF,IF,PDL开关(0/1),通道间隔` |
| 第 2 行 | 原始文件前缀 | 不含 `-ODD.csv` / `-EVEN.csv` 后缀的完整路径前缀（路径或文件名中含 10 位产品 PN） |
| 第 3 行 | 模板目录（MIMS 写入） | 第 1 字段由 MIMS 写入、**程序忽略**；第 2 字段为 ExcelTemplate 备用路径 |
| 第 4 行 | 产品类型 | `>2` 时启用 Mux/Demux 选项 |
| 第 5 行（可选） | 产品 PN 备用 | 第 2 行无法解析 PN 时使用 |

#### 内置 PN → C/L band 映射（不依赖第 3 行第 1 字段）

MIMS 可能将第 3 行第 1 字段写死为 `...\1831760177` 等路径，**程序完全忽略**。每次 **扫描** / **导表** 前：

1. 重读第 2 行（及可选第 5 行），解析 10 位 PN
2. 按内置规则选脚本与 ini：

| PN | Band | 脚本 | ini |
|----|------|------|-----|
| **1831532952** | L band | `1831532952_script_*` | `1831532952.ini`（`Vaue=0`） |
| **其余全部** | C band | `1834650041_script_*` | `1834650041.ini`（`Vaue=190000`） |

3. ExcelTemplate 目录：**优先** `..\config\ExcelTemplate`（相对 exe）；不存在时再用第 3 行第 2 字段

示例（4 行即可，第 3 行第 1 字段内容无关紧要）：

```text
3.CD导表的模板和脚本,C:\Public-T\...\1831760177,C:\Public-T\...\ExcelTemplate,
```

或开发环境：

```text
3.CD导表的模板和脚本,ignored,..\config\ExcelTemplate,
```

路径相对 `CD_Scan.exe` 所在目录解析，不依赖 MIMS 启动时的当前工作目录。

#### ini 加载（`LoadCalGhz`）

按**脚本前缀**加载，不使用 `{PN}.ini`（避免 NAS 上错误的 `1831760177.ini` 干扰 C band）：

- L band → `1831532952.ini`
- C band → `1834650041.ini`
- 文件不存在时使用程序默认（`Vaue=190000`，`BandMode=C`）

**L band 导表行数**：脚本第 2 行信道号为频率×100（如 `1841` = 184.1 THz）；`Vaue=0` 时 `CalcRealLineCount` 使用 `(last-first)/(nchspace/100)+1`（例：`1841–1869`、`nchspace=200` → 15 行）。ITU 网格从 `末信道×100 + nchspace` 起算会比脚本范围多 1 个高端点，导表取网格**末尾** `nRealLine` 行，使最低频对齐 `firstCh`（含 L42 等末信道）。

开发样例见 [`CD_Scan/Temp_config.txt`](CD_Scan/Temp_config.txt)。

### 运行时文件（exe 同目录）

| 文件 | 说明 |
|------|------|
| `CD_IP.txt` | CD 测试仪 IP 地址 |
| `Temp_testfile.txt` | 当前测试原始 CSV 路径（扫描/导表时自动写入） |
| `parameter.ini` | 预匹配合格阈值，节名 `8048M_1` / `8048M_2` |
| `amalgamate.ini` | COER/DCM 文件路径与数量配置 |
| `NO_pair.ini` | 预匹配失败时记录的 COER 序列号 |
| `{产品号}.ini` | 频段锚点（部署在 ExcelTemplate；程序按脚本前缀加载，见上表） |

#### `{产品号}.ini` — `[CalGhz]` 节

| 键 | 说明 | 默认值 |
|----|------|--------|
| `Vaue` | L 段 / 单 C 段 ITU 锚点（×100 内部单位，如 `190000` = 1900 THz 基准） | `190000` |
| `CalGhzC` | C+L 产品时 C 段锚点 | `190000` |
| `LastChC` | C+L 产品时 C 段末信道（脚本第 2 行末信道为 L 段） | `60.5` |
| `BandMode` | `C` 纯 C band；`CL` 同一 CSV 导出 L+C 两段 | `C` |

样例：

- C band：[`CFOI050100OPL03.ini`](CFOI050100OPL03.ini)
- C+L band：[`CFOI050CM0ADV01.ini`](CFOI050CM0ADV01.ini)

C+L 导表时，ITU 网格先按 L 锚点生成 L 段信道，再按 C 锚点生成 C 段信道，依次写入同一 `*-导表.csv`。

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
- 混合工位由内置 PN 规则选脚本，**勿依赖** Temp_config 第 3 行第 1 字段；ExcelTemplate 部署 `1831532952_*` + `1834650041_*` + 对应 ini
- 导表前若检测到扫描频段与所选脚本前缀不一致（C 脚本 + L 频段数据等），程序会弹出警告
- 若目标 `*-导表.csv` 已存在，程序会自动备份为 `*-导表.csv_N.csv`，不直接覆盖
- 仓库根目录大量 `*.csv`、`*_script_*.txt` 为测试样例，非程序配置模板
- C+L 产品原始 CSV 须覆盖 L band 频率（约 `185xxx–190xxx`）与 C band（约 `191xxx–196xxx`）；仅 C band 扫描波长时 L 段指标会异常
- 仪器相关功能需在安装 Agilent SDK 且仪器联网的环境下验证

## 源码结构

| 文件 | 职责 |
|------|------|
| [`CD_Scan/Form1.cs`](CD_Scan/Form1.cs) | UI、仪器连接、扫描、导表、预匹配 |
| [`CD_Scan/Calc.cs`](CD_Scan/Calc.cs) | 数据读取、`ILCalc` 指标计算、INI 读写 |
| [`CD_Scan/LineFit.cs`](CD_Scan/LineFit.cs) | 最小二乘与直线拟合 |
| [`CD_Scan/Program.cs`](CD_Scan/Program.cs) | 程序入口 |
