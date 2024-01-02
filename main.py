import mediapipe as mp
import cv2
import numpy as np
import threading
import time
import pickle
import struct
import os

model_path = os.path.join(os.path.dirname(__file__), 'model5.p')
model_dict = pickle.load(open(model_path, 'rb'))
model = model_dict['model']

DEBUG = True
MODEL_COMPLEXITY = 0

labels_dict = {
    0: 'A', 1: 'B', 2: 'C', 3: 'D', 4: 'E', 5: 'F', 6: 'G', 7: 'H', 8: 'I',
    9: 'J', 10: 'K', 11: 'L', 12: 'M', 13: 'N', 14: 'O', 15: 'P', 16: 'Q',
    17: 'R', 18: 'S', 19: 'T', 20: 'U', 21: 'V', 22: 'W', 23: 'X', 24: 'Y',
    25: 'Z', 26: 'GOJO', 27: 'Atas', 28: 'Kanan', 29: 'Kiri', 30: 'Bawah'
}

class CaptureThread(threading.Thread):
    cap = None
    ret = None
    frame = None
    isRunning = False
    counter = 0
    timer = 0.0

    def run(self):
        self.cap = cv2.VideoCapture(0)
        print("Opened Capture")
        while True:
            self.ret, self.frame = self.cap.read()
            self.isRunning = True
            if DEBUG:
                self.counter += 1
                if time.time() - self.timer >= 3:
                    print("Capture FPS: ", self.counter / (time.time() - self.timer))
                    self.counter = 0
                    self.timer = time.time()

class HandThread(threading.Thread):
    data = ""
    dirty = True

    def run(self):
        mp_drawing = mp.solutions.drawing_utils
        mp_hands = mp.solutions.hands

        capture = CaptureThread()
        capture.start()

        with mp_hands.Hands(min_detection_confidence=0.3, min_tracking_confidence=0.5, model_complexity=MODEL_COMPLEXITY) as hands:
            while capture.isRunning == False:
                print("Waiting for capture")
                time.sleep(0.5)
            print("Beginning capture")

            while capture.cap.isOpened():
                ret = capture.ret
                frame = capture.frame

                image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
                image.flags.writeable = DEBUG

                results = hands.process(image)
                image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

                if results.multi_hand_landmarks:
                    for num, hand in enumerate(results.multi_hand_landmarks):
                        mp_drawing.draw_landmarks(
                            image, hand, mp_hands.HAND_CONNECTIONS,
                            mp_drawing.DrawingSpec(color=(121, 22, 76), thickness=2, circle_radius=4),
                            mp_drawing.DrawingSpec(color=(250, 44, 250), thickness=2, circle_radius=2)
                        )

                    # Kode untuk klasifikasi
                    data_aux = []
                    x_ = []
                    y_ = []

                    for hand_landmarks in results.multi_hand_landmarks:
                        for i in range(len(hand_landmarks.landmark)):
                            x = hand_landmarks.landmark[i].x
                            y = hand_landmarks.landmark[i].y

                            x_.append(x)
                            y_.append(y)

                        for i in range(len(hand_landmarks.landmark)):
                            x = hand_landmarks.landmark[i].x
                            y = hand_landmarks.landmark[i].y
                            data_aux.append(x - min(x_))
                            data_aux.append(y - min(y_))

                    data_aux_sliced = data_aux[:42]
                    prediction = model.predict([np.asarray(data_aux_sliced)])
                    predicted_character = labels_dict[int(prediction[0])]

                    cv2.putText(image, predicted_character, (10, 40), cv2.FONT_HERSHEY_SIMPLEX, 1.3, (0, 0, 255), 3, cv2.LINE_AA)
                    self.data = f"Right|Predicted|{predicted_character}" 
                    self.dirty = True
                    print(self.data)

                if DEBUG:
                    cv2.imshow('Hand Tracking', image)
                    if cv2.waitKey(5) & 0xFF == ord('q'):
                        break

        capture.cap.release()
        cv2.destroyAllWindows()

if __name__ == "__main__":
    hand_thread = HandThread()
    hand_thread.start()

    f = open(r'\\.\pipe\UnityMediaPipeHands', 'r+b', 0)

    while True:
        if hand_thread.dirty:
            s = hand_thread.data.encode('utf-8')
            f.write(struct.pack('I', len(s)) + s)   
            f.seek(0)
            hand_thread.dirty = False

        time.sleep(16/1000)
    
    hand_thread.join()
