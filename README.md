Here is a professional `README.md` for your project. You can copy-paste this directly into your GitHub repository or project folder.

---

# Unity ML-Agents: RollerBall 🤖📦

A reinforcement learning simulation built with **Unity 6** and **ML-Agents**. This project demonstrates a simple AI agent (a blue cube) learning to navigate a plane to collect a randomly spawning target (a red sphere) using the **PPO (Proximal Policy Optimization)** algorithm.

## 📋 Prerequisites

Before running this project, ensure you have the following installed:

- **Unity Hub & Editor:** Unity 2022.3 or Unity 6 (Recommended).
- **Python:** Version 3.9.x (Crucial for stability on Mac Silicon).
- **ML-Agents Package:** `com.unity.ml-agents` (Install via Unity Package Manager).

## 🛠️ Installation & Setup

### 1. Python Environment (Mac M1/M2/M3/M4)

Due to dependency issues on Apple Silicon, use this specific installation order:

```bash
# 1. Create and activate a virtual environment (Python 3.9)
uv venv --python 3.9
source .venv/bin/activate

# 2. Install dependencies with specific version fixes
uv pip install "setuptools<70" wheel
uv pip install "protobuf==3.20.3"
uv pip install six

# 3. Install ML-Agents
uv pip install mlagents

```

or

```
uv pip install -r requirements.txt
```

### 2. Unity Project Setup

1. Open the project in **Unity**.
2. Open **Window > Package Manager**.
3. Add package by name: `com.unity.ml-agents`.
4. **Important:** Go to **Edit > Project Settings > Player** and set **Active Input Handling** to **Both** (or "Input System Package (New)" if using the updated script).

## 🚀 How to Run

### Mode A: Manual Control (Heuristic)

Use your keyboard to test the agent's movement and physics logic.

1. Select the **Agent** in the Hierarchy.
2. In the `Behavior Parameters` component, set **Behavior Type** to `Heuristic Only`.
3. Press **Play**.
4. **Controls:**

- `W` / `S`: Move Forward/Back (Z-Axis).
- `A` / `D`: Move Left/Right (X-Axis).

### Mode B: Training the AI

1. Set **Behavior Type** back to `Default`.
2. Open your terminal and activate the environment.
3. Run the training command:

```bash
mlagents-learn config.yaml --run-id=RollerBall_Train1

```

4. When you see the Unity logo in the terminal, press **Play** in the Unity Editor.
5. Watch the `Mean Reward` increase in the terminal.

### Mode C: Inference (Running the Trained Brain)

1. Locate the `.onnx` file in `results/RollerBall_Train1/`.
2. Drag the file into your Unity **Project** window.
3. Select the **Agent**.
4. Drag the `.onnx` file into the **Model** slot of the `Behavior Parameters` component.
5. Press **Play**. The AI will run automatically.

### Mode D: Tensorboard Results

1. Run the following in the Python folder with environment activated

```
tensorboard --logdir results
```

## 🧠 Technical Details

### The Agent (`RollerAgent.cs`)

- **Observations (6 Inputs):**
- Agent Position `(x, y, z)`
- Target Position `(x, y, z)`

- **Actions (2 Continuous Outputs):**
- Force X (Left/Right)
- Force Z (Forward/Back)

- **Rewards:**
- `+1.0`: Reaching the Target.
- `End Episode`: Falling off the platform ().

### Hyperparameters (`config.yaml`)

Optimized for simple continuous control tasks:

- **Batch Size:** 64 (Frequent updates).
- **Buffer Size:** 2048.
- **Learning Rate:** 3.0e-4.
- **Hidden Units:** 128 (Small brain size for faster inference).

## 📂 File Structure

```text
.
├── Assets/
│   ├── Python/                 # Python training environment (ML-Agents)
│   │   ├── .venv/              # Virtual environment (Python 3.9)
│   │   ├── config.yaml         # Training configuration
│   │   ├── requirements.txt    # Python dependencies (mlagents, torch, etc.)
│   │   └── results/            # Training output (.onnx models located here)
│   ├── Scripts/
│   │   ├── TrainingAgent.cs    # Logic for learning (includes rewards/resets)
│   │   ├── GameAgent.cs        # Optimized version for final gameplay/inference
│   │   └── CameraController.cs # New Input System fly-camera (WASD + Arrows)
│   ├── Prefabs/
│   │   ├── TrainingGround.prefab # Reusable training unit for parallel learning
│   │   └── GameGround.prefab     # Clean environment for final deployment
│   ├── Scenes/
│   │   ├── TrainingGround.unity  # Scene for mass-parallel training
│   │   └── GameGround.unity      # Single-agent gameplay scene
│   ├── Materials/              # Player.mat (Blue) and Target.mat (Orange/Red)
└── README.md                   # Project documentation

```

## 🐛 Troubleshooting

- **"Command not found: mlagents-learn"**: You forgot to activate the venv (`source .venv/bin/activate`).
- **"NullReferenceException"**: The `Target` transform slot is empty in the Inspector. Drag the Red Sphere into it.
- **Agent flips over**: Freeze Rotation X and Z in the Rigidbody constraints.
