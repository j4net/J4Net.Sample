package ru.sample;

public class Sample {
	private int x;
	private int y;
	private String str;
	
	public Sample(int x, int y, String str) {
		this.x = x;
		this.y = y;
		this.str = str;
	}
	
	public int inc(int a) {
		if (a <= 0) {
			throw new RuntimeException("Number should be bigger than zero");
		}
		
		System.out.println(this.str);
		
		return a + 1;
	}
	
	public static void print(String text) {
		System.out.println(text);
	}
}