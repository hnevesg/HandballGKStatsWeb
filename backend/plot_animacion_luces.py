import pandas as pd
import matplotlib.pyplot as plt
import matplotlib.animation as animation

id_to_light_sequence = {
    0: (0, 0),  
    1: (0, 1),
    2: (0, 2),
    3: (1, 0),
    4: (1, 1),
    5: (1, 2),
    6: (2, 0),
    7: (2, 1),
    8: (2, 2),
}

def create_animation(light_sequence):
    light_sequence['grid_x'] = light_sequence['ID_Luz'].map(lambda x: id_to_light_sequence[x][0])
    light_sequence['grid_y'] = light_sequence['ID_Luz'].map(lambda x: id_to_light_sequence[x][1])
    light_sequence = light_sequence.sort_values('Tiempo').reset_index()

    # Set up plot
    fig, ax = plt.subplots(figsize=(6, 6))
    ax.set_xlim(-0.5, 2.5)
    ax.set_ylim(-0.5, 2.5)
    ax.grid(True)
    ax.set_xticks([0, 1, 2])
    ax.set_xticklabels(['0', '3', '6'])
    ax.set_xlabel("Light ID", labelpad=15)
    ax.set_yticks([0, 1, 2])
    ax.set_yticklabels(['0', '1', '2'])
    ax.set_ylabel("Light ID", rotation=90, labelpad=15)

    right_ax = ax.twinx()

    # Align it with the main plot's Y-axis
    right_ax.set_ylim(ax.get_ylim())
    right_ax.set_yticks([0, 1, 2])
    right_ax.set_yticklabels(['6', '7', '8'])

    ax.set_title("Touch Sequence Over Time")

    past_dots = ax.scatter([], [], s=100, c='blue', edgecolor='black')
    current_dot = ax.scatter([], [], s=200, c='red', edgecolor='black')

    # Animation update
    def update(frame):
        current = light_sequence.iloc[frame]
        past = light_sequence.iloc[:frame]

        past_dots.set_offsets(past[['grid_x', 'grid_y']].values)
        current_dot.set_offsets([[current['grid_x'], current['grid_y']]])

        ax.set_title(f"Time: {current['Tiempo']:.2f} s — Light ID: {current['ID_Luz']}")
        return past_dots, current_dot

    ani = animation.FuncAnimation(
        fig, update, frames=len(light_sequence), interval=300, blit=False, repeat=False
    )

    return ani
